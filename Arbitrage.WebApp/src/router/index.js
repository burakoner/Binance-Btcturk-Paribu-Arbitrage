import Vue from 'vue';
import VueRouter from 'vue-router';
import store from '@/store';
import * as AppConfig from '@/config/AppConfig';

Vue.use(VueRouter);

const routes = [
  {
    path: '/',
    redirect: 'dashboard',
  },
  
  {
    path: '/auth/login',
    name: 'auth-login',
    component: () => import('@/views/Auth/Login.vue'),
    meta: { layout: 'blank', title: 'Login', needs: { login: false, moderator: false, superadmin: false } },
  },
  {
    path: '/auth/logout',
    name: 'auth-logout',
    component: () => import('@/views/Auth/Logout.vue'),
    meta: { layout: 'blank', title: 'Logout', needs: { login: false, moderator: false, superadmin: false } },
  },

  {
    path: '/dashboard',
    name: 'dashboard',
    component: () => import('@/views/Dashboard/Index.vue'),
    meta: { title: 'Dashboard', needs: { login: true, moderator: false, superadmin: false } },
  },

  {
    path: '/account',
    name: 'account',
    component: () => import('@/views/Account/Index.vue'),
    meta: { title: 'Account Settings', needs: { login: true, moderator: false, superadmin: false } },
  },

  {
    path: '/opps/:mode',
    name: 'opps',
    component: () => import('@/views/Opps/Index.vue'),
    meta: { title: 'Fırsatlar', needs: { login: true, moderator: false, superadmin: false } },
  },

  {
    path: '/redir',
    name: 'redir',
    meta: { title: 'Redirecting...' },
    component: () => import('@/views/Redirect.vue'),
  },

  {
    path: '/error-404',
    name: 'error-404',
    component: () => import('@/views/Error.vue'),
    meta: { layout: 'blank' },
  },

  {
    path: '*',
    // Birinci Seçenek
    // redirect: 'error-404',

    // İkinci Seçenek
    component: () => import('@/views/Error.vue'),
    meta: { layout: 'blank' },
  },

];

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes,
});

// This callback runs before every route change, including on page load.
router.beforeEach((to, from, next) => {
  // Prevent NavigationDuplicated
  // console.log("beforeEach");
  // console.log(from);
  // console.log(to);

  // This goes through the matched routes from last to first, finding the closest route with a title.
  // eg. if we have /some/deep/nested/route and /some, /deep, and /nested have titles, nested's will be chosen.
  // If a route with a title was found, set the document (page) title to that value.
  const nearestWithTitle = to.matched
    .slice()
    .reverse()
    .find((r) => r.meta && r.meta.title);
    if (nearestWithTitle) document.title = nearestWithTitle.meta.title + ' - Arbitraj Paneli';
    else document.title = 'Arbitraj Paneli';

  // Does page need user login?
  const nearestWithNeeds = to.matched
    .slice()
    .reverse()
    .find((r) => r.meta.needs && r.meta.needs.login);
  if (nearestWithNeeds) {
    const needLogin = nearestWithNeeds.meta.needs.login;

    if (needLogin) {
      const clientTime = new Date().getTime();
      const isLoggedIn = store.state.session.loggedIn;

      if (!isLoggedIn) {
        localStorage.removeItem('session');
        return next({ name: 'auth-login' });
      }
    }
  }

  // Return Next
  next();
});

export default router;
