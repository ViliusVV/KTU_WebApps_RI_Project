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
    return fetch(`${Config.apiUrl}/sensors`, requestOptions.get())
        .then(handleResponse);
}

function getById(id) {
    return fetch(`${Config.apiUrl}/sensors/${id}`, requestOptions.get())
        .then(handleResponse);
}

function create(participant){
    clearIds(participant);
    return fetch(`${Config.apiUrl}/sensors/`, requestOptions.post(participant))
    .then(handleResponse);
}

function deleteItem(id){
    return fetch(`${Config.apiUrl}/sensors/${id}`, requestOptions.delete())
    .then(handleResponse);
}

function clearIds(obj){
    delete obj.id;
    delete obj.createdAt;
}