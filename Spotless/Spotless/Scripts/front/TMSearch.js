;
(function ($) {
    $.TMSearch = function (o) {
        var defaults = {
            form: '.search-form',
            input: '.search-form_input',
            toggle: '.search-form_toggle',
            liveout: '.search-form_liveout',
            out: '#search-results',
            scope: 1,

            handler: 'bat/SearchHandler.php'
        }

        var options = $.extend({}, defaults, o);

        var $form = $(options.form),
            $formIsStuck= $('.isStuck .search-form'),
            $input = $(options.input, $form),
            $liveout = $(options.liveout, $form),
            $toggle = $(options.toggle),
            $out = $(options.out);

        // Search toggle
        if ($toggle.length > 0) {
            $toggle.click(function () {
                if ($(this).parent('.isStuck').hasClass('isStuck')) {
                    if ($form.parent('.isStuck').hasClass('isStuck')){
                        $formIsStuck.toggleClass('on');
                        $(document).on('scroll',function(){
                            if ($formIsStuck.offset().top < 100) {

                                // if (!$toggle.hasClass('active')) {
                                //     $(this).parents().eq(options.scope).find(options.form).find('input').val('').focus();
                                // }
                                $toggle.removeClass('active');
                                $formIsStuck.removeClass('on');
                                return false;
                            }
                        });

                        if (!$toggle.hasClass('active')) {
                            $(this).parents().eq(options.scope).find(options.form).find('input').val('').focus();
                        }

                        $toggle.toggleClass('active');
                        return false;
                    }
                } else if ($(this).parent('.isStuck').hasClass('isStuck') == false){
                        $(this).parent().find($form).toggleClass('on');
                        $(this).parent().find($toggle).toggleClass('active');
                        return false;
                }
            });

            $(document).on('scroll',function(){
                if (($('html').scrollTop() > 100) || ($('body').scrollTop()  > 100)) {
                    $('header .stuck_container:first-child .search-form').removeClass('on')
                    $('header .stuck_container:first-child .search-form_toggle').removeClass('active');
                    // return false;
                }
                
            });

            $(document).click(function (e) {
                if ($toggle.hasClass('active') && e.target.className.indexOf(options.form.substr(1, options.form.length - 1))) {
                    $toggle.removeClass('active');
                    $form.removeClass('on');
                }
            });
        }

        // Live Search
        if ($('html').hasClass('desktop')) {
            $input.on("keyup", function () {
                $.get(
                    options.handler,
                    {
                        s: $(this).val(),
                        liveSearch: "true",
                        dataType: "html"
                    },
                    onSuccess
                );
                function onSuccess(data) {
                    $liveout.html(data);
                }
            });

            $input.on('focusout', function () {
                setTimeout(function () {
                    $liveout.html('');
                }, 300);
            })
        }

        // Frame Search
        if ($out.length > 0) {
            $out.height(0);
            var s = location.search.replace(/^\?.*s=([^&]+)/, '$1'),
                ifr = $('<iframe width="100%" height="100%" frameborder="0" marginheight="0" marginwidth="0" allowTransparency="true"></iframe>')

            if ($out.length) {
                ifr.attr({src: options.handler + '?s=' + s}).appendTo($out), $input.val(decodeURI(s));
            }

            window._resize = function (h) {
                $out.height(h)
            }
        }
    }

})(jQuery);

$(document).ready(function () {
    $.TMSearch();
});
