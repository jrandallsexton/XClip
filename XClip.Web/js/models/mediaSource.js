var mediaSource = function(data) {
    this.id = null;
    this.uId = null;
    this.fName = null;
    this.fExt = null;
    this.mimeType = null;

    if (data != null && data != undefined) {
        this.id = data.id;
        this.uId = data.uId;
        this.fName = data.fName;
        this.fExt = data.fExt;
        this.mimeType = data.mimeType;
    };
};
