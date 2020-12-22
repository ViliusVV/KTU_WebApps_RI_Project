<template>
  <v-app id="inspire">
    <v-navigation-drawer
      v-model="drawer"
      app
    >
      <v-list-item>
        <v-list-item-content>
          <v-list-item-title class="title">
            Menu
          </v-list-item-title>
        </v-list-item-content>
      </v-list-item>

      <v-divider></v-divider>

      <v-list
        dense
        nav
      >
        <v-list-item
          v-for="item in allowedLinks"
          :key="item.title"
          :to="item.to"
          link
        >
            <v-list-item-icon>
              <v-icon>{{ item.icon }}</v-icon>
            </v-list-item-icon>

            <v-list-item-content>
              <v-list-item-title>{{ item.title }}</v-list-item-title>
            </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>

    <v-app-bar app class="header">
      <v-app-bar-nav-icon @click="drawer = !drawer"></v-app-bar-nav-icon>

      <v-toolbar-title>Robots-Intelect Dashboard</v-toolbar-title>
      <div v-if="isLoggedIn" class="name-role">
        {{currentUser.name}} {{currentUser.surname}} - {{currentUser.role}}
      </div>
      <v-spacer></v-spacer>
      <v-btn
          color="primary"
          to="/auth" 
          v-show="!isLoggedIn"
        >
          Login
      </v-btn>
      <v-btn
          color="primary"
          @click="logout"
          v-show="isLoggedIn"
        >
          Logout
      </v-btn>
    </v-app-bar>

    <v-main>
      <router-view></router-view>
    </v-main>

    <v-bottom-navigation
      absolute
      horizontal
      class="footer"
      style="z-index: 1000"
    >
      <v-btn
        color="deep-purple accent-4"
        text
      >
        <span>Robots' Intelect - 2020 </span>
      </v-btn>
    </v-bottom-navigation>
  </v-app>
</template>

<script>
import HelloWorld from './components/HelloWorld';

import Login from './components/Login';

import { authService } from './services/authService';
import router from './router'
import { userService } from './services/userService'
import { Role } from './helpers/role'

export default {
  // NAME
  name: 'App',
  // COMPONENTS
  components: {
    HelloWorld,
    Login,
  },
  // DATA
  data: () => ({
    drawer: null,
    currentUser: null,
    isLoggedIn: false,
    items: [
          { title: 'Main page', icon: 'mdi-home', to: '/', },
          { title: 'Scoreboard Advanced', icon: 'mdi-counter', to: '/scoreboard' },
          { title: 'Scoreboard LEGO', icon: 'mdi-counter', to: '/scoreboardlego' },
          { title: 'Users', icon: 'mdi-account-box-multiple', to: '/users', auth: [Role.Admin]},
          { title: 'Participants', icon: 'mdi-account-multiple', to: '/participants', auth: [Role.Admin, Role.Referee]},
          { title: 'Teams', icon: 'mdi-hexagon-multiple', to: '/teams', auth: [Role.Admin, Role.Referee]},
          { title: 'Robots', icon: 'mdi-robot', to: '/robots', auth: [Role.Admin, Role.Referee]},
          { title: 'About RI', icon: 'mdi-information', to: '/about',}
    ]
  }),
  computed: {
    allowedLinks(){
      const allow = []
      this.items.forEach(item => {
          if(item.auth === undefined){
            allow.push(item);
          }else{
            if(this.currentUser  && item.auth.includes(this.currentUser.role)){
              allow.push(item);
            }
          }
      });

      return allow;
    }
  },
  created () {
      authService.currentUser.subscribe(x => this.currentUser = x);
  },
  methods: {
      logout () {
          console.log('logging out by button')
          authService.logout();
          router.push('/auth');
      },
      isAdmin () {
          console.log("Is admin");
          console.log(this.currentUser && this.currentUser.role === Role.Admin);
          return this.currentUser && this.currentUser.role === Role.Admin;
      }
  },
  watch: {
    // whenever question changes, this function will run
    currentUser: function (currentUser, oldCurrentUser) {
      if(currentUser !== null){
        this.isLoggedIn = true;
      }else{
        this.isLoggedIn = false;
      }
    }
  },
};
</script>

<style scoped>
  @import './style/theme.css';
</style>
