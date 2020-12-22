import { Config } from '../helpers/config';
import { handleResponse, requestOptions } from '../helpers/utils';

export const teamService = {
    getAll,
    getById,
    create,
    deleteItem,
    update
};

function getAll() {
    return fetch(`${Config.apiUrl}/teams`, requestOptions.get())
        .then(handleResponse);
}

function getById(id) {
    return fetch(`${Config.apiUrl}/teams/${id}`, requestOptions.get())
        .then(handleResponse);
}

function create(team){
    clearIds(team);
    return fetch(`${Config.apiUrl}/teams/`, requestOptions.post(team))
    .then(handleResponse);
}

function update(id, team){
    clearIds(team);
    return fetch(`${Config.apiUrl}/teams/${id}`, requestOptions.put(team))
    .then(handleResponse);
}

function deleteItem(id){
    return fetch(`${Config.apiUrl}/teams/${id}`, requestOptions.delete())
    .then(handleResponse);
}

function clearIds(obj){
    delete obj.id;
    delete obj.createdAt;
}