var API_BASE = 'http://localhost:5016/api';
var Api = {

    // GET
    get: function (url, onSuccess, onError) {
        $.ajax({
            url: API_BASE + url,
            type: 'GET',
            xhrFields: { withCredentials: true },
            contentType: 'application/json',
            success: function (data) {
                if (typeof onSuccess === 'function') onSuccess(data);
            },
            error: function (xhr) {
                Api._handleError(xhr, onError);
            }
        });
    },

    // POST 
    post: function (url, body, onSuccess, onError) {
        $.ajax({
            url: API_BASE + url,
            type: 'POST',
            xhrFields: { withCredentials: true },
            contentType: 'application/json',
            data: JSON.stringify(body),
            success: function (data) {
                if (typeof onSuccess === 'function') onSuccess(data);
            },
            error: function (xhr) {
                Api._handleError(xhr, onError);
            }
        });
    },

    // PUT
    put: function (url, body, onSuccess, onError) {
        $.ajax({
            url: API_BASE + url,
            type: 'PUT',
            xhrFields: { withCredentials: true },
            contentType: 'application/json',
            data: JSON.stringify(body),
            success: function (data) {
                if (typeof onSuccess === 'function') onSuccess(data);
            },
            error: function (xhr) {
                Api._handleError(xhr, onError);
            }
        });
    },

    // DELETE
    del: function (url, onSuccess, onError) {
        $.ajax({
            url: API_BASE + url,
            type: 'DELETE',
            xhrFields: { withCredentials: true },
            contentType: 'application/json',
            success: function (data) {
                if (typeof onSuccess === 'function') onSuccess(data);
            },
            error: function (xhr) {
                Api._handleError(xhr, onError);
            }
        });
    },

    // Error handler
    _handleError: function (xhr, onError) {
        var msg = 'An error occurred.';
        try {
            var json = JSON.parse(xhr.responseText);
            msg = json.message || json.title || xhr.responseText || msg;
        } catch (e) {
            msg = xhr.responseText || msg;
        }
        console.error('API Error [' + xhr.status + ']:', msg);
        if (typeof onError === 'function') {
            onError(msg, xhr.status);
        } else {
            Helpers.toast(msg, 'err');
        }
    }
};
