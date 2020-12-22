import { Config } from '../helpers/config';
import { handleResponse, requestOptions } from '../helpers/utils';

export const userService = {
    getAll,
    getById,
    create,
    update,
    deleteItem
};

function getAll() {
    return fetch(`${Config.apiUrl}/users`, requestOptions.get())
        .then(handleResponse);
}

function getById(id) {
    return fetch(`${Config.apiUrl}/users/${id}`, requestOptions.get())
        .then(handleResponse);
}

function create(user){
    clearIds(user);
    return fetch(`${Config.apiUrl}/users`, requestOptions.post(user))
    .then(handleResponse);
}

function update(id, user){
    clearIds(user);
    return fetch(`${Config.apiUrl}/users/${id}`, requestOptions.put(user))
    .then(handleResponse);
}

function deleteItem(id){
    return fetch(`${Config.apiUrl}/users/${id}`, requestOptions.delete())
    .then(handleResponse);
}

function clearIds(obj){
    delete obj.id;
    delete obj.createdAt;
}