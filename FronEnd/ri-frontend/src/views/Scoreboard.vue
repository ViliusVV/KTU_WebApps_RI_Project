<template>
    <div>
        <h1>Home</h1>
        <p>You're logged in with Vue.js & JWT!!</p>
        <p>Your role is: <strong>{{currentUser.role}}</strong>.</p>
        <p>This page can be accessed by all authenticated users.</p>
        <div>
            Current user from secure api end point:
            <ul v-if="userFromApi">
                <li>{{userFromApi.id}} {{userFromApi.username}} {{userFromApi.contactInfo.email}}</li>
            </ul>
        </div>
    </div>
</template>

<script>
import { userService } from '../services/userService';
import { authService } from '../services/authService';


export default {
    data () {
        return {
            currentUser: authService.currentUserValue,
            currentRole: authService.currentUserValue,
            userFromApi: null
        };
    },
    created () {
        userService.getById(this.currentUser.id).then(user => this.userFromApi = user);
    }
};
</script>