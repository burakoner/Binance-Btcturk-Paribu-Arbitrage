import Vue from 'vue';
import Vuex from 'vuex';
import { Utils } from '@/modules/Utils';

Vue.use(Vuex);

const sessionBase = {
  loggedIn: false,
  accessToken: '',
};

let sessionObject = sessionBase;

try {
  const sessionJson = JSON.parse(localStorage.getItem('session'));
  const isLoggedIn = sessionJson.loggedIn;
  const securityCheck = Utils.checkLSSS(sessionJson);

  if (isLoggedIn && securityCheck) {
    sessionObject = {
      loggedIn: sessionJson.loggedIn,
      accessToken: sessionJson.accessToken,
    };
  } else {
    localStorage.removeItem('session');
  }
} catch (err) {
  localStorage.removeItem('session');
} finally {
}

export default new Vuex.Store({
  state: {
    session: sessionObject,
    userData: {},
    userFavs: [],
    oppsDict: {},
    oppsList: [],
    oppsFavs: [],

    oppsB2TC: [],
    oppsB2PC: [],
    oppsT2BC: [],
    oppsT2PC: [],
    oppsP2BC: [],
    oppsP2TC: [],

    oppsB2TX: [],
    oppsB2PX: [],
    oppsT2BX: [],
    oppsT2PX: [],
    oppsP2BX: [],
    oppsP2TX: [],

    oppsMarkets: [],
  },

  actions: {},

  modules: {},

  getters: {
    getSession: (state) => () => {
      return state.session;
    },
    getUserData: (state) => () => {
      return state.userData;
    },
  },

  mutations: {
    SESSION_LOGIN: (state, payload) => {
      // Add Signature
      payload.signature = Utils.createLSSS(payload);

      console.log(payload);
      // Store Payload
      state.session = payload;
      // Vue.set(state.session, 0, payload);

      // Set Local Storage
      localStorage.setItem('session', JSON.stringify(payload));
    },

    SESSION_LOGOUT: (state) => {
      // Store Payload
      state.session = sessionBase;
      state.userData = {};
      state.userFavs = [];
      state.oppsDict = {};
      state.oppsList = [];
      state.oppsFavs = [];

      state.oppsB2TC = [];
      state.oppsB2PC = [];
      state.oppsT2BC = [];
      state.oppsT2PC = [];
      state.oppsP2BC = [];
      state.oppsP2TC = [];

      state.oppsB2TX = [];
      state.oppsB2PX = [];
      state.oppsT2BX = [];
      state.oppsT2PX = [];
      state.oppsP2BX = [];
      state.oppsP2TX = [];

      state.oppsMarkets = [];

      // Remove Local Storage
      localStorage.removeItem('session');
    },

    SET_USER_DATA: (state, payload) => {
      state.userData = payload;
      // Vue.set(state.userData, payload);
    },

    SET_USER_FAVS: (state, payload) => {
      state.userFavs = payload;
      state.oppsFavs = [];
      // Vue.set(state.userData, payload);
    },

    SET_OPPS_MARKETS: (state, payload) => {
      state.oppsMarkets = payload;
    },

    UPDATE_OPPS: (state, payload) => {
      // Prepare
      let index = -1;

      // Dictionary
      /* Kullanmıyorum diye kaldırdım. Yoksa çalışıyor
      if (!state.oppsDict.hasOwnProperty(payload.Symbol)) state.oppsDict[payload.Symbol] = {};
      state.oppsDict[payload.Symbol] = payload;
      */

      // List
      /* Kullanmıyorum diye kaldırdım. Yoksa çalışıyor
      index = state.oppsList.findIndex((x) => x.Symbol == payload.Symbol);
      if (index >= 0) state.oppsList[index] = payload;
      else state.oppsList.push(payload);
      */

      // Favs
      if (state.userFavs) {
        for (let i = 0; i < state.userFavs.length; i++) {
          /* ******************************************************************************************
           ** CLASSIC
           ********************************************************************************************* */
          if (state.userFavs[i].MARKET_ID == payload.Market.ID && state.userFavs[i].MODE == 1) {
            // Binance to BtcTurk
            if (payload.BinanceToBtcTurkClassic) {
              if (state.userFavs[i].EXC_SENDER == 1 && state.userFavs[i].EXC_RECIPIENT == 3) {
                index = state.oppsFavs.findIndex((x) => x.Symbol == payload.Symbol && x.Sender == 1 && x.Recipient == 3 && x.Mode == 1);
                if (index >= 0) state.oppsFavs[index] = payload.BinanceToBtcTurkClassic;
                else state.oppsFavs.push(payload.BinanceToBtcTurkClassic);
              }
            }

            // Binance to Paribu
            if (payload.BinanceToParibuClassic) {
              if (state.userFavs[i].EXC_SENDER == 1 && state.userFavs[i].EXC_RECIPIENT == 2) {
                index = state.oppsFavs.findIndex((x) => x.Symbol == payload.Symbol && x.Sender == 1 && x.Recipient == 2 && x.Mode == 1);
                if (index >= 0) state.oppsFavs[index] = payload.BinanceToParibuClassic;
                else state.oppsFavs.push(payload.BinanceToParibuClassic);
              }
            }

            // BtcTurk to Binance
            if (payload.BtcTurkToBinanceClassic) {
              if (state.userFavs[i].EXC_SENDER == 3 && state.userFavs[i].EXC_RECIPIENT == 1) {
                index = state.oppsFavs.findIndex((x) => x.Symbol == payload.Symbol && x.Sender == 3 && x.Recipient == 1 && x.Mode == 1);
                if (index >= 0) state.oppsFavs[index] = payload.BtcTurkToBinanceClassic;
                else state.oppsFavs.push(payload.BtcTurkToBinanceClassic);
              }
            }

            // BtcTurk to Paribu
            if (payload.BtcTurkToParibuClassic) {
              if (state.userFavs[i].EXC_SENDER == 3 && state.userFavs[i].EXC_RECIPIENT == 2) {
                index = state.oppsFavs.findIndex((x) => x.Symbol == payload.Symbol && x.Sender == 3 && x.Recipient == 2 && x.Mode == 1);
                if (index >= 0) state.oppsFavs[index] = payload.BtcTurkToParibuClassic;
                else state.oppsFavs.push(payload.BtcTurkToParibuClassic);
              }
            }

            // Paribu to Binance
            if (payload.ParibuToBinanceClassic) {
              if (state.userFavs[i].EXC_SENDER == 2 && state.userFavs[i].EXC_RECIPIENT == 1) {
                index = state.oppsFavs.findIndex((x) => x.Symbol == payload.Symbol && x.Sender == 2 && x.Recipient == 1 && x.Mode == 1);
                if (index >= 0) state.oppsFavs[index] = payload.ParibuToBinanceClassic;
                else state.oppsFavs.push(payload.ParibuToBinanceClassic);
              }
            }

            // Paribu to BtcTurk
            if (payload.ParibuToBtcTurkClassic) {
              if (state.userFavs[i].EXC_SENDER == 2 && state.userFavs[i].EXC_RECIPIENT == 3) {
                index = state.oppsFavs.findIndex((x) => x.Symbol == payload.Symbol && x.Sender == 2 && x.Recipient == 3 && x.Mode == 1);
                if (index >= 0) state.oppsFavs[index] = payload.ParibuToBtcTurkClassic;
                else state.oppsFavs.push(payload.ParibuToBtcTurkClassic);
              }
            }
          }

          /* ******************************************************************************************
           ** CROSS
           ********************************************************************************************* */
          if (state.userFavs[i].MARKET_ID == payload.Market.ID && state.userFavs[i].MODE == 2) {
            // Binance to BtcTurk
            if (payload.BinanceToBtcTurkCross) {
              if (state.userFavs[i].EXC_SENDER == 1 && state.userFavs[i].EXC_RECIPIENT == 3) {
                index = state.oppsFavs.findIndex((x) => x.Symbol == payload.Symbol && x.Sender == 1 && x.Recipient == 3 && x.Mode == 2);
                if (index >= 0) state.oppsFavs[index] = payload.BinanceToBtcTurkCross;
                else state.oppsFavs.push(payload.BinanceToBtcTurkCross);
              }
            }

            // Binance to Paribu
            if (payload.BinanceToParibuCross) {
              if (state.userFavs[i].EXC_SENDER == 1 && state.userFavs[i].EXC_RECIPIENT == 2) {
                index = state.oppsFavs.findIndex((x) => x.Symbol == payload.Symbol && x.Sender == 1 && x.Recipient == 2 && x.Mode == 2);
                if (index >= 0) state.oppsFavs[index] = payload.BinanceToParibuCross;
                else state.oppsFavs.push(payload.BinanceToParibuCross);
              }
            }

            // BtcTurk to Binance
            if (payload.BtcTurkToBinanceCross) {
              if (state.userFavs[i].EXC_SENDER == 3 && state.userFavs[i].EXC_RECIPIENT == 1) {
                index = state.oppsFavs.findIndex((x) => x.Symbol == payload.Symbol && x.Sender == 3 && x.Recipient == 1 && x.Mode == 2);
                if (index >= 0) state.oppsFavs[index] = payload.BtcTurkToBinanceCross;
                else state.oppsFavs.push(payload.BtcTurkToBinanceCross);
              }
            }

            // BtcTurk to Paribu
            if (payload.BtcTurkToParibuCross) {
              if (state.userFavs[i].EXC_SENDER == 3 && state.userFavs[i].EXC_RECIPIENT == 2) {
                index = state.oppsFavs.findIndex((x) => x.Symbol == payload.Symbol && x.Sender == 3 && x.Recipient == 2 && x.Mode == 2);
                if (index >= 0) state.oppsFavs[index] = payload.BtcTurkToParibuCross;
                else state.oppsFavs.push(payload.BtcTurkToParibuCross);
              }
            }

            // Paribu to Binance
            if (payload.ParibuToBinanceCross) {
              if (state.userFavs[i].EXC_SENDER == 2 && state.userFavs[i].EXC_RECIPIENT == 1) {
                index = state.oppsFavs.findIndex((x) => x.Symbol == payload.Symbol && x.Sender == 2 && x.Recipient == 1 && x.Mode == 2);
                if (index >= 0) state.oppsFavs[index] = payload.ParibuToBinanceCross;
                else state.oppsFavs.push(payload.ParibuToBinanceCross);
              }
            }

            // Paribu to BtcTurk
            if (payload.ParibuToBtcTurkCross) {
              if (state.userFavs[i].EXC_SENDER == 2 && state.userFavs[i].EXC_RECIPIENT == 3) {
                index = state.oppsFavs.findIndex((x) => x.Symbol == payload.Symbol && x.Sender == 2 && x.Recipient == 3 && x.Mode == 2);
                if (index >= 0) state.oppsFavs[index] = payload.ParibuToBtcTurkCross;
                else state.oppsFavs.push(payload.ParibuToBtcTurkCross);
              }
            }
          }
        }
      }

      /* ******************************************************************************************
       ** CLASSIC
       ********************************************************************************************* */
      // Binance to BtcTurk
      if (payload.BinanceToBtcTurkClassic) {
        index = state.oppsB2TC.findIndex((x) => x.Symbol == payload.Symbol);
        if (index >= 0) state.oppsB2TC[index] = payload.BinanceToBtcTurkClassic;
        else state.oppsB2TC.push(payload.BinanceToBtcTurkClassic);
      }

      // Binance to Paribu
      if (payload.BinanceToParibuClassic) {
        index = state.oppsB2PC.findIndex((x) => x.Symbol == payload.Symbol);
        if (index >= 0) state.oppsB2PC[index] = payload.BinanceToParibuClassic;
        else state.oppsB2PC.push(payload.BinanceToParibuClassic);
      }

      // BtcTurk to Binance
      if (payload.BtcTurkToBinanceClassic) {
        index = state.oppsT2BC.findIndex((x) => x.Symbol == payload.Symbol);
        if (index >= 0) state.oppsT2BC[index] = payload.BtcTurkToBinanceClassic;
        else state.oppsT2BC.push(payload.BtcTurkToBinanceClassic);
      }

      // BtcTurk to Paribu
      if (payload.BtcTurkToParibuClassic) {
        index = state.oppsT2PC.findIndex((x) => x.Symbol == payload.Symbol);
        if (index >= 0) state.oppsT2PC[index] = payload.BtcTurkToParibuClassic;
        else state.oppsT2PC.push(payload.BtcTurkToParibuClassic);
      }

      // Paribu to Binance
      if (payload.ParibuToBinanceClassic) {
        index = state.oppsP2BC.findIndex((x) => x.Symbol == payload.Symbol);
        if (index >= 0) state.oppsP2BC[index] = payload.ParibuToBinanceClassic;
        else state.oppsP2BC.push(payload.ParibuToBinanceClassic);
      }

      // Paribu to BtcTurk
      if (payload.ParibuToBtcTurkClassic) {
        index = state.oppsP2TC.findIndex((x) => x.Symbol == payload.Symbol);
        if (index >= 0) state.oppsP2TC[index] = payload.ParibuToBtcTurkClassic;
        else state.oppsP2TC.push(payload.ParibuToBtcTurkClassic);
      }

      /* ******************************************************************************************
       ** CROSS
       ********************************************************************************************* */
      // Binance to BtcTurk
      if (payload.BinanceToBtcTurkCross) {
        index = state.oppsB2TX.findIndex((x) => x.Symbol == payload.Symbol);
        if (index >= 0) state.oppsB2TX[index] = payload.BinanceToBtcTurkCross;
        else state.oppsB2TX.push(payload.BinanceToBtcTurkCross);
      }

      // Binance to Paribu
      if (payload.BinanceToParibuCross) {
        index = state.oppsB2PX.findIndex((x) => x.Symbol == payload.Symbol);
        if (index >= 0) state.oppsB2PX[index] = payload.BinanceToParibuCross;
        else state.oppsB2PX.push(payload.BinanceToParibuCross);
      }

      // BtcTurk to Binance
      if (payload.BtcTurkToBinanceCross) {
        index = state.oppsT2BX.findIndex((x) => x.Symbol == payload.Symbol);
        if (index >= 0) state.oppsT2BX[index] = payload.BtcTurkToBinanceCross;
        else state.oppsT2BX.push(payload.BtcTurkToBinanceCross);
      }

      // BtcTurk to Paribu
      if (payload.BtcTurkToParibuCross) {
        index = state.oppsT2PX.findIndex((x) => x.Symbol == payload.Symbol);
        if (index >= 0) state.oppsT2PX[index] = payload.BtcTurkToParibuCross;
        else state.oppsT2PX.push(payload.BtcTurkToParibuCross);
      }

      // Paribu to Binance
      if (payload.ParibuToBinanceCross) {
        index = state.oppsP2BX.findIndex((x) => x.Symbol == payload.Symbol);
        if (index >= 0) state.oppsP2BX[index] = payload.ParibuToBinanceCross;
        else state.oppsP2BX.push(payload.ParibuToBinanceCross);
      }

      // Paribu to BtcTurk
      if (payload.ParibuToBtcTurkCross) {
        index = state.oppsP2TX.findIndex((x) => x.Symbol == payload.Symbol);
        if (index >= 0) state.oppsP2TX[index] = payload.ParibuToBtcTurkCross;
        else state.oppsP2TX.push(payload.ParibuToBtcTurkCross);
      }
    },
  },
});
