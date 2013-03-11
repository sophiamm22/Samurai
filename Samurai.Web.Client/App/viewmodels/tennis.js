define(['services/logger'], function (logger) {
    var vm = {
        activate: activate,
        title: 'Tennis'
    };

    return vm;

    //#region Internal Methods
    function activate() {
        logger.log('Tennis View Activated', null, 'tennis', true);
        return true;
    }
    //#endregion
});