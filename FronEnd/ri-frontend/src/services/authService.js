import { BehaviorSubject } from 'rxjs';

import { Config } from '../helpers/config';

import { requestOptions, handleResponse } from '../helpers/utils';
import jwtDecode from 'jwt-decode';

const currentUserSubject = new BehaviorSubject(JSON.parse(localStorage.getItem('currentUser')));

export const authService = {
    login,
    logout,
    currentUser: currentUserSubject.asObservable(),
    get currentUserValue () { return currentUserSubject.value },
};

function login(username, password) {
    return fetch(`${Config.apiUrl}/auth/authenticate`, requestOptions.post({ username, password }))
        .then(handleResponse)
        .then(user => {
            // store user details and jwt token in local storage to keep user logged in between page refreshes
            let payload = jwtDecode(user.jwtToken);
            user.role = payload.role;
            localStorage.setItem('currentUser', JSON.stringify(user));
            console.log(`Logged in user: ${JSON.stringify(user)}`)
            currentUserSubject.next(user);

            return user;
        });
}

function logout() {
    // remove user from local storage to log user out
    localStorage.removeItem('currentUser');

    currentUserSubject.next(null);
}