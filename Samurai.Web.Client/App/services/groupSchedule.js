define(['config'],
  function (config) {
    var schedulesToHeadings = function (footballSchedules, tennisSchedules) {
      var hashCompetition = config.hashes.todayByCompetition;
      var hashSport = config.hashes.todayBySport;

      var sched = [];

      sched.push({
        caption: 'Today',
        sport: '',
        competition: '',
        hash: config.hashes.today,
        isSelected: ko.observable()
      });

      if (footballSchedules.length > 0) {
        sched.push({
          caption: 'Football',
          sport: 'Football',
          competition: '',
          hash: hashSport + '/football',
          isSelected: ko.observable()
        });
        reduceSchedules(footballSchedules, sched, 'Football');
      }
      if (tennisSchedules.length > 0) {
        sched.push({
          caption: 'Tennis',
          sport: 'Tennis',
          competition: '',
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
        var competitionSlug = convertToSlug(competition);
        var hashCompetition = config.hashes.todayByCompetition;

        if (!memo.index[competition]) {
          memo.index[competition] = true;

          memo.sched.push({
            caption: competition,
            sport: sport,
            competition: competition,
            hash: hashCompetition + '/' + competitionSlug,
            isSelected: ko.observable()
          });
        }
        return memo;
      }, { index: {}, sched: collection });
    }

    function convertToSlug(text) {
      return text
               .toLowerCase()
               .replace(/[^\w ]+/g, '')
               .replace(/ +/g, '-');
    }

  });