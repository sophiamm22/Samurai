define(['config', 'services/utils'],
  function (config, utils) {
    var schedulesToHeadings = function (footballSchedules, tennisSchedules) {
      var hashCompetition = config.hashes.todayByCompetition;
      var hashSport = config.hashes.todayBySport;

      var sched = [];

      sched.push({
        caption: 'Today',
        sport: '',
        sportSlug: '',
        competition: '',
        competitionSlug: '',
        hash: config.hashes.today,
        isSelected: ko.observable()
      });

      if (footballSchedules.length > 0) {
        sched.push({
          caption: 'Football',
          sport: 'Football',
          sportSlug: 'football',
          competition: '',
          competitionSlug: '',
          hash: hashSport + '/football',
          isSelected: ko.observable()
        });
        reduceSchedules(footballSchedules, sched, 'Football');
      }
      if (tennisSchedules.length > 0) {
        sched.push({
          caption: 'Tennis',
          sport: 'Tennis',
          sportSlug: 'tennis',
          competition: '',
          competitionSlug: '',
          hash: hashSport + '/tennis',
          isSelected: ko.observable()
        });
        reduceSchedules(tennisSchedules, sched, 'Tennis');
      }

      return sched;

    };

    return {
      schedulesToHeadings: schedulesToHeadings
    };

    function reduceSchedules(schedules, collection, sport) {
      _.reduce(schedules, function (memo, sched) {
        var competition = sched.tournament ? sched.tournament() : sched.league();
        var sportSlug = utils.convertToSlug(sport);
        var competitionSlug = utils.convertToSlug(competition);
        var hashCompetition = config.hashes.todayByCompetition;

        if (!memo.index[competition]) {
          memo.index[competition] = true;

          memo.sched.push({
            caption: competition,
            sportSlug: sportSlug,
            sport: sport,
            competitionSlug: competitionSlug,
            competition: competition,
            hash: hashCompetition + '/' + competitionSlug,
            isSelected: ko.observable()
          });
        }
        return memo;
      }, { index: {}, sched: collection });
    }
  });