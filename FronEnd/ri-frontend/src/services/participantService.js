import { Config } from '../helpers/config';
import { handleResponse, requestOptions } from '../helpers/utils';

export const participantService = {
    getAll,
    getById,
    create,
    deleteItem,
    update
};

function getAll() {
    return fetch(`${Config.apiUrl}/participants`, requestOptions.get())
        .then(handleResponse);
}

function getById(id) {
    return fetch(`${Config.apiUrl}/participants/${id}`, requestOptions.get())
        .then(handleResponse);
}

function create(participant){
    clearIds(participant);
    return fetch(`${Config.apiUrl}/participants/`, requestOptions.post(participant))
    .then(handleResponse);
}

function update(id, participant){
    clearIds(participant);
    return fetch(`${Config.apiUrl}/participants/${id}`, requestOptions.put(participant))
    .then(handleResponse);
}

function deleteItem(id){
    return fetch(`${Config.apiUrl}/participants/${id}`, requestOptions.delete())
    .then(handleResponse);
}

function clearIds(obj){
    delete obj.id;
    delete obj.createdAt;
}