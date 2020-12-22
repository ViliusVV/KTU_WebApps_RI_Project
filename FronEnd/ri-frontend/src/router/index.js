import Vue from 'vue'
import VueRouter from 'vue-router'
import views from '../views'
import { Role } from  '../helpers/role'
import { authService } from "../services/authService";

Vue.use(VueRouter)

const routes = [
  {
    path: '/',
    name: 'Home',
    component: views.Home,
  },
  {
    path: '/about',
    name: 'About',
    component: views.About,
  },
  {
    path: '/users',
    name: 'Users',
    component: views.Users,
    meta: { auth: [Role.Admin] },
  },
  {
    path: '/teams',
    name: 'Teams',
    component: views.Teams,
    meta: { auth: [Role.Admin, Role.Referee] },
  },
  {
    path: '/participants',
    name: 'Participants',
    component: views.Participants,
    meta: { auth: [Role.Admin, Role.Referee] },
  },
  {
    path: '/robots',
    name: 'Robots',
    component: views.Robots,
    meta: { auth: [Role.Admin, Role.Referee] },
  },
  {
    path: '/scoreboard',
    name: 'Scoreboard',
    component: views.Scoreboard,
  },
  {
    path: '/scoreboardlego',
    name: 'ScoreboardLego',
    component: views.ScoreboardLego,
  },
  { 
    path: '/auth', 
    component: views.LoginPage 
  },
  {
    path: '/permissionDenied403',
    name: 'Error 403',
    component: views.PermissionDenied403,
  }
]

const router = new VueRouter({
  routes
})


router.beforeEach((to, from, next) => {
  const { auth } = to.meta;
  const currentUser = authService.currentUserValue;

  if(auth){
    if (!currentUser) {
      // not logged in so redirect to login page with the return url
      return next({ path: '/auth', query: { returnUrl: to.path } });
    }


    // check if route is restricted by role
    if (auth.length && !auth.includes(currentUser.role)) {
      // role not authorised so redirect to home page
      return next({ path: '/permissionDenied403' });
    }
  }

  next();
})

export default router

