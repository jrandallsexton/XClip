
var apiWrapper = new function () {

    this.rootPath = "http://localhost/xclipApi/api/";

    this.getData = function(cacheKey, url, callback) {
        var data = this.appDecache(cacheKey, true);
        if (data !== null) {
            callback(data);
            return;
        }

        this.ajaxGet(url, function (values) {
            apiWrapper.appCache(cacheKey, values, true);
            callback(values);
        }, true);
    };

    this.getRandomMedia = function(collectionId, callback) {
        return this.ajaxGet(this.rootPath + 'media/' + collectionId + '/random', callback, true);
    };

    this.getVideoTags = function(mediaId, callback) {
        return this.ajaxGet(this.rootPath + '/tags?mediaId=' + mediaId, callback, true);
    }

    this.getTags = function(callback) {
        return this.ajaxGet(this.rootPath + 'tags', callback, true);
    };

    this.skipMediaSource = function(mediaId, callback) {
        return this.ajaxPut(this.rootPath + 'media/' + mediaId, null, callback, true);
    };
    
    this.mediaSourceDelete = function(mediaId, callback) {
        
    };
    
    function batchSave(batch) {
        return ajaxPost(URL_REST_SVC + '/batch', JSON.stringify(batch));
    }

    this.getSkills = function(personId, callback) {
        var url = this.rootPath + "api/person/" + personId + "/skill"
        this.ajaxGet(url, function (values) {
            callback(values);
        }, true);
    };

    this.getTitles = function (callback) {
        this.getData("getTitles", this.rootPath + "api/title/lookup", callback); };

    this.deletePosition = function(personId, positionId, callback) {
        var url = this.rootPath + "api/person/" + personId + "/position/" + positionId;
        this.ajaxDelete(url, function (values) {
            callback(values);
        }, true);
    };

    this.savePosition = function(position, callback) {
        if (position.PositionId == null) {
            var url = this.rootPath + "api/person/" + position.PersonId + "/position";
            this.ajaxPost(url, position, true, function (values) {
                callback(values);
            }, true);
        }
        else {
            var url = this.rootPath + "api/person/" + position.PersonId + "/position/" + position.Id;
            this.ajaxPut(url, position, true, function (values) {
                callback(values);
            }, true);
        }
    };

    this.saveSkills = function(personId, skills, callback) {
        var url = this.rootPath + "api/person/" + personId + "/skill";
        this.ajaxPut(url, skills, true, function (values) {
            callback(values);
        }, true);
    };

    this.ajaxDelete = function(url, callback, returnData) {
        $.ajax({
            data: {},
            type: "DELETE",
            url: url,
            contentType: "application/json",
            dataType: "json",
            cache: false,
            success: function (result) {
                if (callback != null) {
                    if (returnData) {
                        callback(result);
                    }
                    else {
                        callback();
                    }
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                debugger;
            }
        });
    };

    this.ajaxGet = function(url, callback, returnData) {
        $.ajax({
            data: {},
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            success: function (result) {
                if (callback != null) {
                    if (returnData) {
                        callback(result);
                    }
                    else {
                        callback();
                    }
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                debugger;
            }
        });
    };

    this.ajaxPost = function(url, data, returnData, callback) {
        $.ajax({
            data: JSON.stringify(data),
            type: "POST",
            url: url,
            traditional: true,
            contentType: "application/json",
            dataType: "json",
            cache: false,
            success: function (result) {
                if (callback != null) {
                    if (returnData) {
                        callback(result);
                    }
                    else {
                        callback();
                    }
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                debugger;
            }
        });
    };

    this.ajaxPut = function(url, data, returnData, callback) {
        $.ajax({
            data: JSON.stringify(data),
            type: "PUT",
            url: url,
            traditional: true,
            contentType: "application/json",
            dataType: "json",
            cache: false,
            success: function (result) {
                if (callback != null) {
                    if (returnData) {
                        callback(result);
                    }
                    else {
                        callback();
                    }
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                debugger;
            }
        });
    };

    this.sessionCache = function(key, value, stringify) {
        if (value == null) {
            sessionStorage.setItem(key, null);
        } else {
            if (stringify) {
                sessionStorage.setItem(key, JSON.stringify(value));
            } else {
                sessionStorage.setItem(key, value);
            }
        };
    }

    this.sessionDecache = function(key, parse) {
        var tempVal = sessionStorage.getItem(key);
        if ((tempVal != null) && (tempVal !== '')) {
            if (parse) {
                return JSON.parse(tempVal);
            } else {
                return tempVal;
            }
        }
        return null;
    };

    this.appCache = function(key, value, stringify) {
        if (value == null) {
            localStorage.setItem(key, null);
        } else {
            if (stringify) {
                localStorage.setItem(key, JSON.stringify(value));
            } else {
                localStorage.setItem(key, value);
            }
        }
    };

    this.appDecache = function (key, parse) {
        var tempVal = localStorage.getItem(key);
        if ((tempVal !== null) && (tempVal !== undefined) && (tempVal !== '')) {
            if (parse) {
                return JSON.parse(tempVal);
            } else {
                return tempVal;
            }
        }
        return null;
    };

    this.clearStorage = function() {
        localStorage.clear();
        sessionStorage.clear();
    };
};