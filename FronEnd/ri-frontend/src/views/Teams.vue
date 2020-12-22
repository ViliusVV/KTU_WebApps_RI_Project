<template>
    <div>
        <nav v-if="currentUser" class="navbar navbar-expand navbar-dark bg-dark">
            <div class="navbar-nav">
                <router-link to="/" class="nav-item nav-link">Home</router-link>
                <router-link v-if="isAdmin" to="/admin" class="nav-item nav-link">Admin</router-link>
                <a @click="logout" class="nav-item nav-link">Logout</a>
            </div>
        </nav>
        <div class="jumbotron">
            <div class="container">
                <div class="row">
                    <div class="col-sm-6 offset-sm-3">
                        <router-view></router-view>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { authService } from '../services/authService';
import router from '../router'
import { userService } from '../services/userService'
import { Role } from '../helpers/role'

export default {
    name: 'app',
    data () {
        return {
            currentUser: null
        };
    },
    computed: {
        isAdmin () {
            return this.currentUser && this.currentUser.role === Role.Admin;
        }
    },
    created () {
        authService.currentUser.subscribe(x => this.currentUser = x);
    },
    methods: {
        logout () {
            authService.logout();
            router.push('/login');
        }
    }
};
</script>