// import { authHeader } from '../helpers/utils';
// import router from '../router/'

// export const userService = {
//     login,
//     logout,
//     getAll
// };

// function login(username, password) {
//     const requestOptions = {
//         method: 'POST',
//         headers: { 'Content-Type': 'application/json' },
//         body: JSON.stringify({ Username: username, Password: password })
//     };

//     return fetch(`http://underpoweredserver.tplinkdns.com:21212/api/auth/authenticate`, requestOptions)
//         .then(handleResponse)
//         .then(user => {
//             // login successful if there's a jwt token in the response
//             if (user.jwtToken) {
//                 // store user details and jwt token in local storage to keep user logged in between page refreshes
//                 localStorage.setItem('user', JSON.stringify(user));
//             }

//             return user;
//         });
// }

// function logout() {
//     // remove user from local storage to log user out
//     localStorage.removeItem('user');
// }

// function getAll() {
//     const requestOptions = {
//         method: 'GET',
//         headers: authHeader()
//     };

//     return fetch(`http://underpoweredserver.tplinkdns.com:21212/api/users`, requestOptions).then(handleResponse);
// }

// function handleResponse(response) {
//     return response.text().then(text => {
//         const data = text && JSON.parse(text);
//         if (!response.ok) {
//             if (response.status === 401 || response.status === 403) {
//                 router.push("/permissionDenied403#" + response.status)
//             }

//             const error = (data && data.message) || response.statusText;
//             return Promise.reject(error);
//         }

//         return data;
//     });
// }