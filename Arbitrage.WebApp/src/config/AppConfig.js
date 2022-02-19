export let DOMAIN = "burakoner.com"; // localhost, supernodeonline.com

/* API Definitions */
export let API_URL = 'https://localhost:44303';
export let API_VERSION = '/api/v1/';
export let WSS_URL = 'wss://localhost:44303';
export let WSS_VERSION = '/stream/v1';
export let API_SECURITY_ERRORS_MIN = 1001;
export let API_SECURITY_ERRORS_MAX = 1099;

export let DEFAULT_LANGUAGE = 'tr';

if (DOMAIN == "localhost--") {
    API_URL = 'https://localhost:5303';
    API_VERSION = '/api/v1/';
    WSS_URL = 'wss://localhost:5303';
    WSS_VERSION = '/stream/v1';
}

if (DOMAIN == "burakoner.com") {
    /* API Definitions */
    API_URL = 'https://api.burakoner.com';
    API_VERSION = '/api/v1/';
    WSS_URL = 'wss://api.burakoner.com';
    WSS_VERSION = '/stream/v1';
}