{
  "schema": {
    "namespace": "BreezeSchema.Models",
    "alias": "Self",
    "d4p1:UseStrongSpatialTypes": "false",
    "xmlns:d4p1": "http://schemas.microsoft.com/ado/2009/02/edm/annotation",
    "xmlns": "http://schemas.microsoft.com/ado/2009/11/edm",
    "cSpaceOSpaceMapping": "[[\\"
    BreezeSchema.Models.Fixture\\ ",\\"
    BreezeSchema.Models.Fixture\\ "],[\\"
    BreezeSchema.Models.Prediction\\ ",\\"
    BreezeSchema.Models.Prediction\\ "],[\\"
    BreezeSchema.Models.ScoreLineProbability\\ ",\\"
    BreezeSchema.Models.ScoreLineProbability\\ "],[\\"
    BreezeSchema.Models.Coupon\\ ",\\"
    BreezeSchema.Models.Coupon\\ "],[\\"
    BreezeSchema.Models.Odd\\ ",\\"
    BreezeSchema.Models.Odd\\ "],[\\"
    BreezeSchema.Models.OutcomeProbability\\ ",\\"
    BreezeSchema.Models.OutcomeProbability\\ "]]",
    "entityType": [{
      "name": "Fixture",
      "key": {
        "propertyRef": {
          "name": "Id"
        }
      },
      "property": [{
        "name": "Id",
        "type": "Edm.Int32",
        "nullable": "false"
      }, {
        "name": "League",
        "type": "Edm.String",
        "fixedLength": "false",
        "maxLength": "Max",
        "unicode": "true",
        "nullable": "true"
      }, {
        "name": "Season",
        "type": "Edm.String",
        "fixedLength": "false",
        "maxLength": "Max",
        "unicode": "true",
        "nullable": "true"
      }, {
        "name": "MatchDate",
        "type": "Edm.DateTime",
        "nullable": "false"
      }, {
        "name": "HomeTeam",
        "type": "Edm.String",
        "fixedLength": "false",
        "maxLength": "Max",
        "unicode": "true",
        "nullable": "true"
      }, {
        "name": "AwayTeam",
        "type": "Edm.String",
        "fixedLength": "false",
        "maxLength": "Max",
        "unicode": "true",
        "nullable": "true"
      }, {
        "name": "ScoreLine",
        "type": "Edm.String",
        "fixedLength": "false",
        "maxLength": "Max",
        "unicode": "true",
        "nullable": "true"
      }, {
        "name": "IKTSGameWeek",
        "type": "Edm.Int32",
        "nullable": "true"
      }
      ],
      "navigationProperty": [{
        "name": "Prediction",
        "relationship": "Self.Fixture_Prediction",
        "fromRole": "Fixture_Prediction_Source",
        "toRole": "Fixture_Prediction_Target"
      }, {
        "name": "Coupon",
        "relationship": "Self.Fixture_Coupon",
        "fromRole": "Fixture_Coupon_Source",
        "toRole": "Fixture_Coupon_Target"
      }
      ]
    }, {
      "name": "Prediction",
      "key": {
        "propertyRef": {
          "name": "Id"
        }
      },
      "property": [{
        "name": "Id",
        "type": "Edm.Int32",
        "nullable": "false",
        "d4p1:StoreGeneratedPattern": "Identity"
      }, {
        "name": "MatchId",
        "type": "Edm.Int32",
        "nullable": "false"
      }, {
        "name": "PredictionURL",
        "type": "Edm.String",
        "fixedLength": "false",
        "maxLength": "Max",
        "unicode": "true",
        "nullable": "true"
      }
      ],
      "navigationProperty": [{
        "name": "ScoreLineProbabilities",
        "relationship": "Self.ScoreLineProbability_FootballPrediction",
        "fromRole": "ScoreLineProbability_FootballPrediction_Target",
        "toRole": "ScoreLineProbability_FootballPrediction_Source"
      }, {
        "name": "FootballFixture",
        "relationship": "Self.Fixture_Prediction",
        "fromRole": "Fixture_Prediction_Target",
        "toRole": "Fixture_Prediction_Source"
      }
      ]
    }, {
      "name": "ScoreLineProbability",
      "key": {
        "propertyRef": {
          "name": "Id"
        }
      },
      "property": [{
        "name": "Id",
        "type": "Edm.Int32",
        "nullable": "false",
        "d4p1:StoreGeneratedPattern": "Identity"
      }, {
        "name": "PredictionId",
        "type": "Edm.Int32",
        "nullable": "false"
      }, {
        "name": "ScoreLine",
        "type": "Edm.String",
        "fixedLength": "false",
        "maxLength": "Max",
        "unicode": "true",
        "nullable": "true"
      }, {
        "name": "ScoreLineProb",
        "type": "Edm.Double",
        "nullable": "true"
      }
      ],
      "navigationProperty": {
        "name": "FootballPrediction",
        "relationship": "Self.ScoreLineProbability_FootballPrediction",
        "fromRole": "ScoreLineProbability_FootballPrediction_Source",
        "toRole": "ScoreLineProbability_FootballPrediction_Target"
      }
    }, {
      "name": "Coupon",
      "key": {
        "propertyRef": {
          "name": "Id"
        }
      },
      "property": [{
        "name": "Id",
        "type": "Edm.Int32",
        "nullable": "false",
        "d4p1:StoreGeneratedPattern": "Identity"
      }, {
        "name": "MatchId",
        "type": "Edm.Int32",
        "nullable": "false"
      }
      ],
      "navigationProperty": [{
        "name": "BestOdds",
        "relationship": "Self.Coupon_BestOdds",
        "fromRole": "Coupon_BestOdds_Source",
        "toRole": "Coupon_BestOdds_Target"
      }, {
        "name": "AllOdds",
        "relationship": "Self.Coupon_AllOdds",
        "fromRole": "Coupon_AllOdds_Source",
        "toRole": "Coupon_AllOdds_Target"
      }, {
        "name": "FootballFixture",
        "relationship": "Self.Fixture_Coupon",
        "fromRole": "Fixture_Coupon_Target",
        "toRole": "Fixture_Coupon_Source"
      }
      ]
    }, {
      "name": "Odd",
      "key": {
        "propertyRef": {
          "name": "Id"
        }
      },
      "property": [{
        "name": "Id",
        "type": "Edm.Int32",
        "nullable": "false",
        "d4p1:StoreGeneratedPattern": "Identity"
      }, {
        "name": "CouponId",
        "type": "Edm.Int32",
        "nullable": "false"
      }, {
        "name": "IsBetable",
        "type": "Edm.Boolean",
        "nullable": "false"
      }, {
        "name": "Outcome",
        "type": "Edm.String",
        "fixedLength": "false",
        "maxLength": "Max",
        "unicode": "true",
        "nullable": "true"
      }, {
        "name": "OddBeforeCommission",
        "type": "Edm.Double",
        "nullable": "false"
      }, {
        "name": "CommissionPct",
        "type": "Edm.Double",
        "nullable": "true"
      }, {
        "name": "DecimalOdd",
        "type": "Edm.Double",
        "nullable": "false"
      }, {
        "name": "TimeStamp",
        "type": "Edm.DateTime",
        "nullable": "false"
      }, {
        "name": "Bookmaker",
        "type": "Edm.String",
        "fixedLength": "false",
        "maxLength": "Max",
        "unicode": "true",
        "nullable": "true"
      }, {
        "name": "OddsSource",
        "type": "Edm.String",
        "fixedLength": "false",
        "maxLength": "Max",
        "unicode": "true",
        "nullable": "true"
      }, {
        "name": "ClickThroughURL",
        "type": "Edm.String",
        "fixedLength": "false",
        "maxLength": "Max",
        "unicode": "true",
        "nullable": "true"
      }, {
        "name": "Priority",
        "type": "Edm.Int32",
        "nullable": "false"
      }
      ],
      "navigationProperty": {
        "name": "Coupon",
        "relationship": "Self.Odd_Coupon",
        "fromRole": "Odd_Coupon_Source",
        "toRole": "Odd_Coupon_Target"
      }
    }, {
      "name": "OutcomeProbability",
      "key": {
        "propertyRef": {
          "name": "Id"
        }
      },
      "property": [{
        "name": "Id",
        "type": "Edm.Int32",
        "nullable": "false",
        "d4p1:StoreGeneratedPattern": "Identity"
      }, {
        "name": "Outcome",
        "type": "Edm.String",
        "fixedLength": "false",
        "maxLength": "Max",
        "unicode": "true",
        "nullable": "true"
      }, {
        "name": "OutcomeProb",
        "type": "Edm.Double",
        "nullable": "false"
      }
      ]
    }
    ],
    "association": [{
      "name": "ScoreLineProbability_FootballPrediction",
      "end": [{
        "role": "ScoreLineProbability_FootballPrediction_Source",
        "type": "Edm.Self.ScoreLineProbability",
        "multiplicity": "*"
      }, {
        "role": "ScoreLineProbability_FootballPrediction_Target",
        "type": "Edm.Self.Prediction",
        "multiplicity": "1",
        "onDelete": {
          "action": "Cascade"
        }
      }
      ],
      "referentialConstraint": {
        "principal": {
          "role": "ScoreLineProbability_FootballPrediction_Target",
          "propertyRef": {
            "name": "Id"
          }
        },
        "dependent": {
          "role": "ScoreLineProbability_FootballPrediction_Source",
          "propertyRef": {
            "name": "PredictionId"
          }
        }
      }
    }, {
      "name": "Fixture_Prediction",
      "end": [{
        "role": "Fixture_Prediction_Target",
        "type": "Edm.Self.Prediction",
        "multiplicity": "1"
      }, {
        "role": "Fixture_Prediction_Source",
        "type": "Edm.Self.Fixture",
        "multiplicity": "1"
      }
      ],
      "referentialConstraint": {
        "principal": {
          "role": "Fixture_Prediction_Target",
          "propertyRef": {
            "name": "Id"
          }
        },
        "dependent": {
          "role": "Fixture_Prediction_Source",
          "propertyRef": {
            "name": "Id"
          }
        }
      }
    }, {
      "name": "Odd_Coupon",
      "end": [{
        "role": "Odd_Coupon_Source",
        "type": "Edm.Self.Odd",
        "multiplicity": "*"
      }, {
        "role": "Odd_Coupon_Target",
        "type": "Edm.Self.Coupon",
        "multiplicity": "1",
        "onDelete": {
          "action": "Cascade"
        }
      }
      ],
      "referentialConstraint": {
        "principal": {
          "role": "Odd_Coupon_Target",
          "propertyRef": {
            "name": "Id"
          }
        },
        "dependent": {
          "role": "Odd_Coupon_Source",
          "propertyRef": {
            "name": "CouponId"
          }
        }
      }
    }, {
      "name": "Coupon_BestOdds",
      "end": [{
        "role": "Coupon_BestOdds_Source",
        "type": "Edm.Self.Coupon",
        "multiplicity": "0..1"
      }, {
        "role": "Coupon_BestOdds_Target",
        "type": "Edm.Self.Odd",
        "multiplicity": "*"
      }
      ]
    }, {
      "name": "Coupon_AllOdds",
      "end": [{
        "role": "Coupon_AllOdds_Source",
        "type": "Edm.Self.Coupon",
        "multiplicity": "0..1"
      }, {
        "role": "Coupon_AllOdds_Target",
        "type": "Edm.Self.Odd",
        "multiplicity": "*"
      }
      ]
    }, {
      "name": "Fixture_Coupon",
      "end": [{
        "role": "Fixture_Coupon_Target",
        "type": "Edm.Self.Coupon",
        "multiplicity": "1"
      }, {
        "role": "Fixture_Coupon_Source",
        "type": "Edm.Self.Fixture",
        "multiplicity": "1"
      }
      ],
      "referentialConstraint": {
        "principal": {
          "role": "Fixture_Coupon_Target",
          "propertyRef": {
            "name": "Id"
          }
        },
        "dependent": {
          "role": "Fixture_Coupon_Source",
          "propertyRef": {
            "name": "Id"
          }
        }
      }
    }
    ],
    "entityContainer": {
      "name": "MyDbContext",
      "entitySet": [{
        "name": "Fixtures",
        "entityType": "Self.Fixture"
      }, {
        "name": "Predictions",
        "entityType": "Self.Prediction"
      }, {
        "name": "ScoreLineProbabilities",
        "entityType": "Self.ScoreLineProbability"
      }, {
        "name": "Coupons",
        "entityType": "Self.Coupon"
      }, {
        "name": "Odds",
        "entityType": "Self.Odd"
      }, {
        "name": "OutcomeProbabilities",
        "entityType": "Self.OutcomeProbability"
      }
      ],
      "associationSet": [{
        "name": "ScoreLineProbability_FootballPrediction",
        "association": "Self.ScoreLineProbability_FootballPrediction",
        "end": [{
          "role": "ScoreLineProbability_FootballPrediction_Source",
          "entitySet": "ScoreLineProbabilities"
        }, {
          "role": "ScoreLineProbability_FootballPrediction_Target",
          "entitySet": "Predictions"
        }
        ]
      }, {
        "name": "Fixture_Prediction",
        "association": "Self.Fixture_Prediction",
        "end": [{
          "role": "Fixture_Prediction_Target",
          "entitySet": "Predictions"
        }, {
          "role": "Fixture_Prediction_Source",
          "entitySet": "Fixtures"
        }
        ]
      }, {
        "name": "Odd_Coupon",
        "association": "Self.Odd_Coupon",
        "end": [{
          "role": "Odd_Coupon_Source",
          "entitySet": "Odds"
        }, {
          "role": "Odd_Coupon_Target",
          "entitySet": "Coupons"
        }
        ]
      }, {
        "name": "Coupon_BestOdds",
        "association": "Self.Coupon_BestOdds",
        "end": [{
          "role": "Coupon_BestOdds_Source",
          "entitySet": "Coupons"
        }, {
          "role": "Coupon_BestOdds_Target",
          "entitySet": "Odds"
        }
        ]
      }, {
        "name": "Coupon_AllOdds",
        "association": "Self.Coupon_AllOdds",
        "end": [{
          "role": "Coupon_AllOdds_Source",
          "entitySet": "Coupons"
        }, {
          "role": "Coupon_AllOdds_Target",
          "entitySet": "Odds"
        }
        ]
      }, {
        "name": "Fixture_Coupon",
        "association": "Self.Fixture_Coupon",
        "end": [{
          "role": "Fixture_Coupon_Target",
          "entitySet": "Coupons"
        }, {
          "role": "Fixture_Coupon_Source",
          "entitySet": "Fixtures"
        }
        ]
      }
      ]
    }
  }
}