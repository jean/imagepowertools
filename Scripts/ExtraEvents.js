(function (window, document, undefined) {

    function isString(value) { return typeof value == 'string'; }

    var lowercase = function (string) { return isString(string) ? string.toLowerCase() : string; };


    var SPECIAL_CHARS_REGEXP = /([\:\-\_]+(.))/g;
    var MOZ_HACK_REGEXP = /^moz([A-Z])/;

    /**
     * Converts snake_case to camelCase.
     * Also there is special case for Moz prefix starting with upper case letter.
     * @param name Name to normalize
     */
    function camelCase(name) {
        return name.
          replace(SPECIAL_CHARS_REGEXP, function (_, separator, letter, offset) {
              return offset ? letter.toUpperCase() : letter;
          }).
          replace(MOZ_HACK_REGEXP, 'Moz$1');
    }


    var PREFIX_REGEXP = /^(x[\:\-_]|data[\:\-_])/i;
    function directiveNormalize(name) {
        return camelCase(name.replace(PREFIX_REGEXP, ''));
    }

    var ngEventDirectives = {};
    
    'dragenter dragover dragleave drop'.split(' ').forEach(
      function (name) {
          var directiveName = directiveNormalize('ee-' + name);
          ngEventDirectives[directiveName] = ['$parse', function ($parse) {
              return function (scope, element, attr) {
                  var fn = $parse(attr[directiveName]);
                  element.bind(lowercase(name), function (event) {
                      scope.$apply(function () {
                          event.preventDefault();
                          fn(scope, { $event: event });
                      });
                  });
              };
          }];
      }
    );

    angular.module('ExtraEvents', [])
                .directive(ngEventDirectives);
})(window, document);