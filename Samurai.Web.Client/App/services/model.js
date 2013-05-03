﻿define(['config'],
  function (config) {
    var DT = breeze.DataType;
    var AutoGeneratedKeyType = breeze.AutoGeneratedKeyType;

    var model = {
      configureMetadataStore: configureMetadataStore
    };

    return model;

    function configureMetadataStore(metadataStore) {
      entityTypeInitialiser(metadataStore);
    }


    function entityTypeInitialiser(metadataStore) {
      metadataStore.addEntityType({
        //autoGeneratedKeyType: AutoGeneratedKeyType.Identity,
        shortName: 'FootballMatch',
        namespace: 'Samurai',
        dataProperties: {
          //footballMatchId:  { dataType: DT.Int32, isPartOfKey: true },
          matchIdentifier:  { dataType: DT.String, isPartOfKey: true },
          league:           { dataType: DT.String },
          season:           { dataType: DT.String },
          matchDate:        { dataType: DT.DateTime },
          homeTeam:         { dataType: DT.String },
          awayTeam:         { dataType: DT.String },
          scoreLine:        { dataType: DT.String },
          iktsGameWeek:     { dataType: DT.Int64 }
        },
        navigationProperties: {
          predictions: {
            entityTypeName: 'FootballPredictions',
            isScalar: false,
            associationName: 'FootballMatch_FootballPredictions'
          }
        }
      });

      metadataStore.addEntityType({
        autoGeneratedKeyType: AutoGeneratedKeyType.Identity,
        shortName: 'FootballPrediction',
        namespace: 'Samurai',
        dataProperties: {
          predictionId:     { dataType: DT.Int32, isPartOfKey: true },
          matchIdentifier:  { dataType: DT.String },
          predictionURL:    { dataType: DT.String }
        },
        navigationProperties: {
          footballMatch: {
            entityTypeName:   'FootballMatch', isScalar: true,
            associationName: 'FootballMatch_FootballPredictions',
            foreignKeyNames: ['matchIdentifier']
          }
        }
      });
    }
  });