var batch = function(data) {
    this.sourceId = null;
    this.items = new Array();
    this.tags = new Array();

    if (data != null && data != undefined) {
        this.sourceId = data.sourceId;
        this.items = data.items;
    }
};