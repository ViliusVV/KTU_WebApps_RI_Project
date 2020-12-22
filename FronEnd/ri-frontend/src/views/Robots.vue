<template>
    <div>
        <h1>Admin</h1>
        <p>This page can only be accessed by administrators.</p>
        <div>
            All users from secure (admin only) api end point:
            <ul v-if="users.length">
                <li v-for="user in users" :key="user.id">
                  {{user.id}} {{user.name}} {{user.contactInfo.email}}
                </li>
            </ul>
        </div>
    </div>
</template>

<script>
import { userService } from '../services/userService';
import { participantService } from '../services/participantService';
import { authService } from '../services/authService';


export default {
    data () {
        return {
            user: authService.currentUserValue,
            users: []
        };
    },
    created () {
        participantService.getAll().then(users => this.users = users);
    }
};
</script>