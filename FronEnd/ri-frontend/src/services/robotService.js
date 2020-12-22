import { Config } from '../helpers/config';
import { handleResponse, requestOptions } from '../helpers/utils';

export const robotService = {
    getAll,
    getById,
    create,
    deleteItem,
    update
};

function getAll() {
    return fetch(`${Config.apiUrl}/robots`, requestOptions.get())
        .then(handleResponse);
}

function getById(id) {
    return fetch(`${Config.apiUrl}/robots/${id}`, requestOptions.get())
        .then(handleResponse);
}

function create(robot){
    clearIds(robot);
    return fetch(`${Config.apiUrl}/robots/`, requestOptions.post(robot))
    .then(handleResponse);
}

function update(id, robot){
    clearIds(robot);
    return fetch(`${Config.apiUrl}/robots/${id}`, requestOptions.put(robot))
    .then(handleResponse);
}

function deleteItem(id){
    return fetch(`${Config.apiUrl}/robots/${id}`, requestOptions.delete())
    .then(handleResponse);
}

function clearIds(obj){
    delete obj.id;
    delete obj.createdAt;
}