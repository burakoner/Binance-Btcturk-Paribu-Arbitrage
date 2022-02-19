import CryptoJS from 'crypto-js';

export const Utils = {
  
  // create unique hash from a string
  unique(str) {
    str = String(str || '')
      .replace(/[\r\n\t\s]+/g, ' ')
      .trim();
    let hash = 5381,
      i = str.length;
    while (--i) hash = (hash * 33) ^ str.charCodeAt(i);
    return 'unq_' + (hash >>> 0);
  },

  // random string for a given length
  randString(length) {
    let chars = 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
    let total = parseInt(length) || 10;
    let output = '';

    while (total) {
      output += chars.charAt(Math.floor(Math.random() * chars.length));
      total--;
    }
    return output;
  },

  // get a unique ID string that uses the current timestamp and a random value
  idString() {
    return (
      Date.now().toString(36) +
      Math.random()
        .toString(36)
        .substr(2, 5)
    ).toUpperCase();
  },

  // Returns if a value is a string
  isString(value) {
    return typeof value === 'string' || value instanceof String;
  },

  // Returns if a value is really a number
  isNumber(value) {
    return typeof value === 'number' && isFinite(value);
  },

  // Returns if a value is an array
  isArray(value) {
    return value && typeof value === 'object' && value.constructor === Array;
  },

  // ES5 actually has a method for this (ie9+)
  // Array.isArray(value);

  // Returns if a value is a function
  isFunction(value) {
    return typeof value === 'function';
  },

  // Returns if a value is an object
  isObject(value) {
    return value && typeof value === 'object' && value.constructor === Object;
  },

  // Returns if a value is null
  isNull(value) {
    return value === null;
  },

  // Returns if a value is undefined
  isUndefined(value) {
    return typeof value === 'undefined';
  },

  // Returns if a value is a boolean
  isBoolean(value) {
    return typeof value === 'boolean';
  },

  // Returns if value is an error object
  isError(value) {
    return value instanceof Error && typeof value.message !== 'undefined';
  },

  // Returns if a value is a regexp
  isRegExp(value) {
    return value && typeof value === 'object' && value.constructor === RegExp;
  },

  // Returns if value is a date object
  isDate(value) {
    return value instanceof Date;
  },

  // Returns if a Symbol
  isSymbol(value) {
    return typeof value === 'symbol';
  },

  // LSSS: LocalStoreSessionSignature
  createLSSS(payload) {
    const signBody = payload.accountId + payload.accessToken;
    const signature = CryptoJS.HmacSHA256(signBody, "SAHddt46qg").toString();
    return signature;
  },

  // LSSS: LocalStoreSessionSignature
  checkLSSS(payload) {
    const signature = this.createLSSS(payload);
    return signature == payload.signature;
  },

  stringFormat(format) {
    var args = Array.prototype.slice.call(arguments, 1);
    return format.replace(/{(\d+)}/g, function(match, number) { 
      return typeof args[number] != 'undefined'
        ? args[number] 
        : match
      ;
    });
  },

   setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays*24*60*60*1000));
    var expires = "expires="+ d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
  },

  getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for(var i = 0; i <ca.length; i++) {
      var c = ca[i];
      while (c.charAt(0) == ' ') {
        c = c.substring(1);
      }
      if (c.indexOf(name) == 0) {
        return c.substring(name.length, c.length);
      }
    }
    return "";
  }

};
