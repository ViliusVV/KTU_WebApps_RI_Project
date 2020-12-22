<template>
    <!-- <div>
        <div class="alert alert-info">
            <strong>Normal User</strong> - U: user P: user<br />
            <strong>Administrator</strong> - U: admin P: admin
        </div>
        <h2>Login</h2>
        <form @submit.prevent="onSubmit">
            <div class="form-group">
                <label for="username">Username</label>
                <input type="text" v-model.trim="$v.username.$model" name="username" class="form-control" :class="{ 'is-invalid': submitted && $v.username.$error }" />
                <div v-if="submitted && !$v.username.required" class="invalid-feedback">Username is required</div>
            </div>
            <div class="form-group">
                <label htmlFor="password">Password</label>
                <input type="password" v-model.trim="$v.password.$model" name="password" class="form-control" :class="{ 'is-invalid': submitted && $v.password.$error }" />
                <div v-if="submitted && !$v.password.required" class="invalid-feedback">Password is required</div>
            </div>
            <div class="form-group">
                <button class="btn btn-primary" :disabled="loading">
                    <span class="spinner-border spinner-border-sm" v-show="loading"></span>
                    <span>Login</span>
                </button>
            </div>
            <div v-if="error" class="alert alert-danger">{{error}}</div>
        </form>
    </div> -->

    <v-container fill-height fluid>
    <v-row align="center"
        justify="center">
        <v-col>  
          <v-card
            class="mx-auto login"
            max-width="600px"
            
          >
            <v-card-title>
              Login
            </v-card-title>

            <v-form
              ref="form"
              v-model="valid"
              lazy-validation
            >
                <v-alert
                  v-if="error"
                  type="error"
                >{{error}}</v-alert>
                <v-text-field
                  v-model="username"
                  :rules="usernameRules"
                  label="Username"
                  required
                ></v-text-field>

                <v-text-field
                  v-model="password"
                  :rules="passwordRules"
                  label="Password"
                  type="password"
                  required
                ></v-text-field>

                <v-btn
                  :disabled="!valid"
                  color="success"
                  @click="onSubmit"
                >
                  Login
                </v-btn>
            </v-form>
          </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>
<script>
import { required } from 'vuelidate/lib/validators';

import router from '../router';
import { authService } from '../services/authService';


  export default {
    data: () => ({
      valid: true,
      username: '',
      usernameRules: [
        v => !!v || 'Username is required',
      ],
      password: '',
      passwordRules: [
        v => !!v || 'Password is required',
      ],
      submitted: false,
      loading: false,
      returnUrl: '',
      error: ''
    }),
    methods: {
      validate () {
        if (this.$refs.form.validate()) {
          this.snackbar = true
        }
      },
      reset () {
        this.$refs.form.reset()
      },
      resetValidation () {
        this.$refs.form.resetValidation()
      },
      onSubmit () {
            this.submitted = true;

            this.loading = true;
            authService.login(this.username, this.password)
                .then(
                    user => router.push(this.returnUrl),
                    error => {
                        this.error = error;
                        this.loading = false;
                    }
                );
        }
    },
    created () {
      // redirect to home if already logged in
      if (authService.currentUserValue) { 
          return router.push('/');
      }

      // get return url from route parameters or default to '/'
      this.returnUrl = this.$route.query.returnUrl || '/';
    },
  }
</script>
