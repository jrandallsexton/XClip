var batchItem = function(data) {
    this.index = null;
    this.start = null;
    this.stop = null;
    this.duration = null;
    this.tags = new Array();

    if (data != null && data != undefined) {
        this.index = data.index;
        this.start = data.start;
        this.stop = data.stop;
        this.duration = data.duration;
        this.tags = data.tags;
    }
};
