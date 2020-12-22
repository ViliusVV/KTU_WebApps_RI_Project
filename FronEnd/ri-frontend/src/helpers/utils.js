import jwt_decode from 'jwt-decode';
import { authService } from '../services/authService'
import router from '../router'


function parseJwt (token) {
    try {
        return JSON.parse(atob(token.split('.')[1]));
      } catch (e) {
        return null;
    }
};

function authHeader() {
    let user = JSON.parse(localStorage.getItem('user'));

    if (user && user.jwtToken) {
        console.log(jwt_decode( user.jwtToken))
        return { 'Authorization': 'Bearer ' + user.jwtToken };
    } else {
        return {};
    }
}


function handleResponse(response) {
    return response.text().then(text => {
        const data = text && JSON.parse(text);
        if (!response.ok) {
            if (response.status === 401 || response.status === 403) {
                router.push("/permissionDenied403#" + response.status)
            }

            const error = (data && data.message) || response.statusText;
            return Promise.reject(error);
        }

        return data;
    });
}

 const requestOptions = {
    get() {
        return {
            method: 'GET',
            ...headers()
        };
    },
    post(body) {
        return {
            method: 'POST',
            ...headers(),
            body: JSON.stringify(body)
        };
    },
    patch(body) {
        return {
            method: 'PATCH',
            ...headers(),
            body: JSON.stringify(body)
        };
    },
    put(body) {
        return {
            method: 'PUT',
            ...headers(),
            body: JSON.stringify(body)
        };
    },
    delete() {
        return {
            method: 'DELETE',
            ...headers()
        };
    }
}

function headers() {
    const currentUser = authService.currentUserValue || {};

    const jwtToken = currentUser.jwtToken;
    const authHeader = currentUser.jwtToken ? { 'Authorization': 'Bearer ' + jwtToken } : {}
    return {
        headers: {
            ...authHeader,
            'Content-Type': 'application/json'
        }
    }
}


export { parseJwt, authHeader, headers, handleResponse, requestOptions }