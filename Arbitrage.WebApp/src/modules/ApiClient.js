import axios from 'axios';
import CryptoJS from 'crypto-js';
import { Utils } from './Utils';
import EventBus from './EventBus';
import * as ApiConstants from './ApiConstants';

export default class ApiClient extends EventBus {
  // #region ctor
  constructor(config, router, store) {
    // Super
    super();

    // App Config
    this._config = config;

    // Vue Router
    this._router = router;

    // Vuex Store
    this._store = store;

    // Rest Api
    this._apiUrl = this._config.API_URL;
    this._apiVersion = this._config.API_VERSION;
    this._apiKey = '';
    this._apiSecret = '';
    this._listenKey = '';
    this._receiveWindow = 60 * 1000;
    this._requestTimeout = 60 * 1000;
    this._signatureHolder = 'x-signature';
    this._accessTokenHolder = 'x-accesstoken';

    // WS Api
    this._wssUrl = this._config.WSS_URL;
    this._wssVersion = this._config.WSS_VERSION;
    this._wssReconnectWait = 10000;
    this._reconnect = {};
    this._timers = {};
    this._socks = {};

    // Session
    this._loggedIn = false;
    this._accessToken = '';

    // Version
    this._serverVersion = null;

    // Time Difference
    this._timeDiff = 0;

    this.serverTime()
      .then((response) => {
        if (response.data.success) {
          let clientTime = new Date().getTime();
          this.setTimeDifference(response.data.data - clientTime);
        }
      })
      .catch((error) => {})
      .finally(() => {});

    // Layouts
    this._blankLayout = null;
    this._mobileLayout = null;
  }
  // #endregion

  // #region Core Getters
  getApiKey(key) {
    return this._apikey || '';
  }

  getApiSecret(secret) {
    return this._apisecret || '';
  }

  getListenKey(key) {
    return this._listenKey || '';
  }

  getReceiveWindow(val) {
    return this._receiveWindow || 60000;
  }

  getRequestTimeout(val) {
    return this._requestTimeout || 60000;
  }

  getTimeDifference(val) {
    return this._timeDiff || 0;
  }
  // #endregion

  // #region Core Setters
  setApiKey(key) {
    this._apikey = String(key || '').trim();
  }

  setApiSecret(secret) {
    this._apisecret = String(secret || '').trim();
  }

  setListenKey(key) {
    this._listenKey = String(key || '').trim();
  }

  setReceiveWindow(val) {
    this._receiveWindow = val || 60000;
  }

  setRequestTimeout(val) {
    this._requestTimeout = val || 60000;
  }

  setTimeDifference(val) {
    this._timeDiff = val || 0;
  }
  // #endregion

  // #region Core Methods
  _request(apiMethod, apiPath, signRequest = false, queryStringDictionary = null, bodyString = '', formData = null) {
    // Method
    apiMethod = apiMethod.toUpperCase();

    // Timestamp
    const timestamp = new Date().getTime() + this._timeDiff;

    // Query String
    let queryString = signRequest ? '?ts=' + timestamp : '';
    if (!Utils.isNull(queryStringDictionary) && Utils.isObject(queryStringDictionary)) {
      for (const [key, value] of Object.entries(queryStringDictionary)) {
        queryString += queryString == '' ? '?' : '&';
        queryString += `${key}=${value}`;
      }
    }

    // Final Url
    let apiFinalUrl = this._apiVersion + apiPath + queryString;

    // Body Data
    if (!Utils.isNull(bodyString)) {
      if (!Utils.isString(bodyString)) {
        bodyString = JSON.stringify(bodyString);
        if (bodyString === 'null') bodyString = '';
      }
    } else {
      bodyString = '';
    }

    // Signature
    let signature = '';
    if (signRequest) {
      let securityKey = timestamp.toString();
      let signBody = apiMethod + apiFinalUrl + bodyString;
      signature = CryptoJS.HmacSHA256(signBody, securityKey.toString()).toString();
    }

    // Headers
    let headers = {};
    if (formData) headers['Content-Type'] = 'multipart/form-data';
    if (signature.length > 0) headers[this._signatureHolder] = signature;
    if (this._accessToken && this._accessToken.length > 0) headers[this._accessTokenHolder] = this._accessToken;

    // Action
    return axios({
      url: this._apiUrl + apiFinalUrl,
      method: apiMethod,
      data: formData ? formData : bodyString,
      headers: headers,
      timeout: this._requestTimeout,
    });
  }

  gateKeeper(req) {
    if (req) {
      req
        .then((response) => {
          var errorCode = response?.data?.error?.code;
          if (errorCode && errorCode >= this._config.API_SECURITY_ERRORS_MIN && errorCode <= this._config.API_SECURITY_ERRORS_MAX) {
            // this._mobileLayout.showApiError(errorCode, this._mobileLayout.$t("api.e" + errorCode));
            this._router.push({ name: 'auth-logout' }).catch((err) => {});
          }
        })
        .catch((error) => {});
    }
  }
  // #endregion

  /* ************************************************************ *
   * REST API
   * ************************************************************ */

  // #region Server Endpoints
  serverPing() {
    return this._request('GET', 'server/ping');
  }

  serverTime() {
    let req = this._request('GET', 'server/time');
    req
      .then((response) => {
        const clientTime = new Date().getTime();
        this._timeDiff = response.data.data - clientTime;
      })
      .catch((error) => {});
    return req;
  }

  serverVersion() {
    let req = this._request('GET', 'server/version');
    req
      .then((response) => {
        if (response.data.success) {
          this._serverVersion = response.data.data;
        }
      })
      .catch((error) => {});
    this.gateKeeper(req);
    return req;
  }
  // #endregion

  // #region Auth Endpoints
  // Epoch Signature
  authRegister(email, password, language, recaptcha, agreement) {
    const data = {
      email: email,
      password: password,
      language: language,
      reCaptcha: recaptcha,
      agreement: agreement,
    };
    return this._request('POST', 'auth/register', true, null, data);
  }

  // Epoch Signature
  authRegisterStatus(refcode) {
    let data = {
      refcode: refcode,
    };
    return this._request('GET', 'auth/register', true, data);
  }

  // Epoch Signature
  authResendEmailCode(email) {
    const data = {
      email: email,
    };
    return this._request('POST', 'auth/register/email-code', true, null, data);
  }

  // Epoch Signature
  authVerifyEmail(refcode, code) {
    let data = {
      refcode: refcode,
      code: code,
    };
    return this._request('POST', 'auth/register/email-confirm', true, null, data);
  }

  // Epoch Signature
  authResetPasswordOne(email) {
    let data = {
      email: email,
    };
    return this._request('POST', 'auth/resetpass-one', true, null, data);
  }

  // Epoch Signature
  authResetPasswordTwo(code, token, password) {
    let data = {
      code: code,
      token: token,
      password: password,
    };
    return this._request('POST', 'auth/resetpass-two', true, null, data);
  }

  // Epoch Signature
  authLogin(email, password) {
    let data = {
      email: email,
      password: password,
    };
    let req = this._request('POST', 'auth/login', true, null, data);
    req
      .then((response) => {
        if (response.data.success) {
          this._loggedIn = true;
          this._accessToken = response.data.data.accessToken;
        }
      })
      .catch((error) => {
        this._loggedIn = false;
        this._accessToken = '';
      });
    return req;
  }

  // Epoch Signature
  authLoginOTP(token, otp) {
    let data = {
      token: token,
      otp: otp,
    };
    let req = this._request('POST', 'auth/login-otp', true, null, data);

    req
      .then((response) => {
        if (response.data.success) {
          this._loggedIn = true;
          this._accessToken = response.data.data.accessToken;
        }
      })
      .catch((error) => {
        this._loggedIn = false;
        this._accessToken = '';
      });
    return req;
  }

  // Access Token Security
  authLogout() {
    let req = this._request('POST', 'auth/logout', true);
    req
      .then((response) => {})
      .catch((error) => {})
      .finally(() => {
        this._loggedIn = false;
        this._accessToken = '';

        this._accountData = {};
        this._expertAccounts = [];
        this._followerAccounts = [];
      });
    return req;
  }

  // Access Token Security
  authCheckSession() {
    let req = this._request('GET', 'auth/session', true);
    this.gateKeeper(req);
    return req;
  }

  // Access Token Security
  authExtendSession() {
    let req = this._request('POST', 'auth/session', true);
    this.gateKeeper(req);
    return req;
  }
  // #endregion

  // #region Account Endpoints
  accountData() {
    let req = this._request('GET', 'account/data', true);
    req
      .then((response) => {
        if (response.data.success) {
          this._accountData = response.data.data;
          this._store.commit('SET_USER_DATA', response.data.data);
        }
      })
      .catch((error) => {});
    this.gateKeeper(req);
    return req;
  }

  accountFavs() {
    let req = this._request('GET', 'account/favs', true);
    req
      .then((response) => {
        if (response.data.success) {
          this._store.commit('SET_USER_FAVS', response.data.data);
        }
      })
      .catch((error) => {});
    this.gateKeeper(req);
    return req;
  }

  setAccountData(email, name, surname, password) {
    let data = {
      email: email,
      name: name,
      surname: surname,
      password: password,
    };
    let req = this._request('POST', 'account/data', true, null, data);
    this.gateKeeper(req);
    return req;
  }

  setAccountApi(data) {
    let req = this._request('POST', 'account/api', true, null, data);
    this.gateKeeper(req);
    return req;
  }

  setAccountTelegram(data) {
    let req = this._request('POST', 'account/telegram', true, null, data);
    this.gateKeeper(req);
    return req;
  }

  oppsMarkets() {
    let req = this._request('GET', 'opps/markets', true);
    req
      .then((response) => {
        if (response.data.success) {
          this._store.commit('SET_OPPS_MARKETS', response.data.data);
        }
      })
      .catch((error) => {});
    this.gateKeeper(req);
    return req;
  }

  setAccountFavs(sender, recipient, mode, dict) {
    let data = {
      data: dict,
      mode: mode,
      sender: sender,
      recipient: recipient,
    };
    let req = this._request('POST', 'account/favs', true, null, data);
    req
    .then((response) => {
      if (response.data.success) {
        this._store.commit('SET_USER_FAVS', response.data.data);
      }
    })
    .catch((error) => {});
  this.gateKeeper(req);
    return req;
  }


  


  
















  accountChangeOtp(otp) {
    let data = {
      otp: otp,
    };
    let req = this._request('POST', 'account/changeotp', true, null, data);
    this.gateKeeper(req);
    return req;
  }

  accountEnableGoogleAuth(code) {
    let data = {
      code: code,
    };
    let req = this._request('POST', 'account/google-auth/enable', true, null, data);
    this.gateKeeper(req);
    return req;
  }

  accountDisableGoogleAuth(code) {
    let data = {
      code: code,
    };
    let req = this._request('POST', 'account/google-auth/disable', true, null, data);
    this.gateKeeper(req);
    return req;
  }

  accountApiKeys() {
    let req = this._request('GET', 'account/apikeys', true);
    this.gateKeeper(req);
    return req;
  }

  accountApiKeysAdd(label, ipWhiteList = '', allowInfo = true, allowWithdrawal = false, allowDust = false, allowSpot = false, allowMargin = false, allowContract = false, allowBinary = false, allowMining = false, allowOTC = false, allowPayment = false) {
    let data = {
      label: label,
      ipWhiteList: ipWhiteList,
      allowInfo: allowInfo,
      allowWithdrawal: allowWithdrawal,
      allowDust: allowDust,
      allowSpot: allowSpot,
      allowMargin: allowMargin,
      allowContract: allowContract,
      allowBinary: allowBinary,
      allowMining: allowMining,
      allowOTC: allowOTC,
      allowPayment: allowPayment,
    };
    let req = this._request('POST', 'account/apikeys/create', true, null, data);
    this.gateKeeper(req);
    return req;
  }

  accountApiKey(apiKey) {
    let req = this._request('GET', 'account/apikeys/' + apiKey, true);
    this.gateKeeper(req);
    return req;
  }

  accountApiKeyEdit(apiKey, label, ipWhiteList = '', allowInfo = true, allowWithdrawal = false, allowDust = false, allowSpot = false, allowMargin = false, allowContract = false, allowBinary = false, allowMining = false, allowOTC = false, allowPayment = false) {
    let data = {
      label: label,
      ipWhiteList: ipWhiteList,
      allowInfo: allowInfo,
      allowWithdrawal: allowWithdrawal,
      allowDust: allowDust,
      allowSpot: allowSpot,
      allowMargin: allowMargin,
      allowContract: allowContract,
      allowBinary: allowBinary,
      allowMining: allowMining,
      allowOTC: allowOTC,
      allowPayment: allowPayment,
    };
    let req = this._request('POST', 'account/apikeys/' + apiKey, true, null, data);
    this.gateKeeper(req);
    return req;
  }

  accountApiKeyDelete(apiKey) {
    let req = this._request('DELETE', 'account/apikeys/' + apiKey, true);
    this.gateKeeper(req);
    return req;
  }
  // #endregion

  /* ************************************************************ *
   * WEBSOCKET API
   * ************************************************************ */

  /**
   * Set socket reconnect boolean for an id
   * @param {string}   id      Sock id ref
   * @param {boolean}  toggle  Toggle
   */
  setReconnect(id, toggle) {
    this._reconnect[id] = toggle ? true : false;
  }

  /**
   * Check reconnect toggle for an id and call a handler function
   * @param {string}    id        Sock id ref
   * @param {function}  callback  Handler function
   */
  checkReconnect(id, callback) {
    if (!this._reconnect[id]) return;
    setTimeout(callback, this._wait);
  }

  /**
   * Start custom timer
   * @param {string}    id        Timer id name
   * @param {number}    time      Interval mils
   * @param {function}  callback  Callback function
   * @param {boolean}   init      Init callback
   */
  startTimer(id, time, callback, init) {
    this.stopTimer(id);
    this._timers[id] = setInterval(callback, time);
    if (init) callback();
  }

  /**
   * Stop custom timer
   * @param {string}  id  Timer id name
   */
  stopTimer(id) {
    if (!id || !this._timers.hasOwnProperty(id)) return;
    clearInterval(this._timers[id]);
    delete this._timers[id];
  }

  /**
   * Create a WebSocket connection
   * @param {string}  id      Ref id name
   * @param {string}  endpoint  Socket endpoint url
   */
  sockConnect(id, endpoint) {
    if (!id || !endpoint) return;
    this.emit(ApiConstants.WSS_SOCKET_ONINIT, endpoint);
    this.sockClose(id);

    if (!('WebSocket' in window)) {
      this.emit(ApiConstants.WSS_SOCKET_ONFAIL, 'This web browser does not have WebSocket support.');
      return false;
    }
    try {
      let ws = new WebSocket(endpoint);
      this._socks[id] = ws;
      return ws;
    } catch (err) {
      let message = String(err.message || 'WebSocket endpoint connection failed for (' + endpoint + ').');
      this.emit(ApiConstants.WSS_SOCKET_ONFAIL, message);
      return false;
    }
  }

  /**
   * Close socket connection and remove it from the list
   * @param {string}  id  Socket id name
   */
  sockClose(id) {
    if (!id || !this._socks.hasOwnProperty(id)) return;
    this.emit(ApiConstants.WSS_SOCKET_ONCLOSE, id);
    this._socks[id].close();
    delete this._socks[id];
  }

  /**
   * Close all active socket connections
   */
  sockCloseAll() {
    Object.keys(this._socks).forEach((id) => this.sockClose(id));
  }

  startOpportunitiesStream(wsunique, reconnect) {
    const wsname = ApiConstants.WSS_OPPS_NAME + '-' + wsunique;

    this.setReconnect(wsname, reconnect || false);
    this.emit(ApiConstants.WSS_OPPS_ONINIT, Date.now());

    const ws = this.sockConnect(wsname, this._wssUrl + this._wssVersion + '?subscribe=Trace.All');
    if (!ws) return this.emit(ApiConstants.WSS_OPPS_ONFAIL, 'Could not connect to live stream API endpoint.');

    ws.addEventListener('open', (e) => {
      this.emit(ApiConstants.WSS_OPPS_ONOPEN, e);
    });

    ws.addEventListener('error', (e) => {
      this.emit(ApiConstants.WSS_OPPS_ONERROR, e);
    });

    ws.addEventListener('close', (e) => {
      this.emit(ApiConstants.WSS_OPPS_ONCLOSE, e);
      this.checkReconnect(wsname, () => this.startOpportunitiesStream(wsunique, reconnect));
    });

    ws.addEventListener('message', (e) => {
      let json = JSON.parse(e.data);
      if (json.c.startsWith('Trace.All')) {
        this.emit(ApiConstants.WSS_OPPS_ONDATA, json);

        const date = new Date(parseInt(json.d.t));
        const opp = json.d;

        this._store.commit('UPDATE_OPPS', opp);
      }
    });
  }

  stopOpportunitiesStream(wsunique) {
    const wsname = ApiConstants.WSS_OPPS_NAME + '-' + wsunique;

    this.setReconnect(wsname, false);
    this.sockClose(wsname);
  }
}
