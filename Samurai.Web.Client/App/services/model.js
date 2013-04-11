define(['config'],
  function (config) {

    var model = {
      configureMetadataStore: configureMetadataStore
    };

    return model;

    function configureMetadataStore(metadataStore) {
      metadataStore.registerEntityTypeCtor(
        'TennisSchedule', null, tennisScheduleInitialiser);
    }

    function tennisScheduleInitialiser(tennisSchedule) {
      //don't actually need this yet.
    }


  });