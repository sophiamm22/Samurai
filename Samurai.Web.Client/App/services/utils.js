define(function () {
  var utils = {
    regExEscape: regExEscape,
    convertToSlug: convertToSlug
  };
  return utils;

  function regExEscape(text) {
    return text.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, "\\$&");
  }

  function convertToSlug(text) {
    return text
             .toLowerCase()
             .replace(/[^\w ]+/g, '')
             .replace(/ +/g, '-');
  }

});