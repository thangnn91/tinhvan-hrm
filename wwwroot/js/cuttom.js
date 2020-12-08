var mobile = false;
var ipad = false;
var ipadNgang = false;
var pc = false;

function testmobile() {
  if ($(window).width() <= 768) {
    mobile = true;
  }
  if (768 < $(window).width() && $(window).width() <= 980) {
    ipad = true;
  }
  if ($(window).width() > 980) {
    pc = true;
  }
  if (980 < $(window).width() && $(window).width() <= 1024) {
    ipadNgang = true;
  }
}
testmobile();
var functionTreetinhvan = function() {
  if ($('.treetinhvan').length > 0) {
    $('.treetinhvan').each(function() {
      var treetinhvan = $(this);
      // <i class="tvc-enlarge" aria-hidden="true"></i> Expand all
      var iconExpand = '<a href="#" class="iconExpand btn btn-outline-grey btn-sm my-0 waves-effect waves-light"><i class="fa fa-caret-down" aria-hidden="true"></i></a>';
      // <i class="tvc-shrink" aria-hidden="true"></i> Collapse all
      var iconCollapse = '<a href="#" class="iconCollapse btn btn-outline-grey btn-sm my-0 waves-effect waves-light"><i class="fa fa-caret-up" aria-hidden="true"></i></a>';
      treetinhvan.treetable({
        expandable: true
      });
      // jQuery(treetinhvan).treetable('expandAll');
      var clickCollapse = $(iconCollapse).appendTo(treetinhvan.find('thead th:first-child'));
      var clickExpand = $(iconExpand).appendTo(treetinhvan.find('thead th:first-child'));
      clickCollapse.hide();
      treetinhvan.find('.iconCollapse').click(function(e) {
        e.preventDefault();
        jQuery(treetinhvan).treetable('collapseAll');
        clickExpand.show();
        clickCollapse.hide();
      });
      treetinhvan.find('.iconExpand').click(function(e) {
        e.preventDefault();
        jQuery(treetinhvan).treetable('expandAll');
        clickExpand.hide();
        clickCollapse.show();
      });
      // Highlight selected row
      treetinhvan.find('tbody').on("mousedown", "tr", function() {
        $(".selected").not(this).removeClass("selected");
        $(this).toggleClass("selected");
      });

      // Drag & Drop Example Code
      $(treetinhvan.find('.file'), treetinhvan.find('.folder')).draggable({
        helper: "clone",
        opacity: .75,
        refreshPositions: true,
        revert: "invalid",
        revertDuration: 300,
        scroll: true
      });

      treetinhvan.find('.folder').each(function() {
        $(this).parents(".treetinhvan tr").droppable({
          accept: ".file, .folder",
          drop: function(e, ui) {
            var droppedEl = ui.draggable.parents("tr");
            treetinhvan.treetable("move", droppedEl.data("ttId"), $(this).data("ttId"));
          },
          hoverClass: "accept",
          over: function(e, ui) {
            var droppedEl = ui.draggable.parents("tr");
            if (this != droppedEl[0] && !$(this).is(".expanded")) {
              treetinhvan.treetable("expandNode", $(this).data("ttId"));
            }
          }
        });
      });
    });
  }
}

function sideNavCuttom() {
  if ($('header .button-collapse').length > 0) {
    var showOverlay = true;
    if (pc) {
      $('header .button-collapse').sideNav({
        edge: 'left', // Choose the horizontal origin
        // closeOnClick: false, // Closes side-nav on &lt;a&gt; clicks, useful for Angular/Meteor
        // breakpoint: 1920, // Breakpoint for button collapse
        MENU_WIDTH: 240, // Width for sidenav
        timeDurationOpen: 200, // Time duration open menu
        timeDurationClose: 200, // Time duration open menu
        timeDurationOverlayOpen: 50, // Time duration open overlay
        timeDurationOverlayClose: 200, // Time duration close overlay
        easingOpen: 'easeOutQuad', // Open animation
        easingClose: 'easeOutCubic', // Close animation
        showOverlay: false, // Display overflay
        showCloseButton: true // Append close button into siednav
      });
    } else {
      $('.button-collapse').sideNav({
        breakpoint: 1240
      });
    }
    ////if ($('.custom-scrollbar').length > 0) {
    ////  if (pc) {
    ////    $("#slide-out,#slide-out .collapsible-body, .menucap3 .ul").mCustomScrollbar({
    ////      theme: "minimal-dark",
    ////      axis: "y",
    ////      scrollbarPosition: "outside",
    ////    });
    ////  }
    ////}
  }
}

function scrollupdate() {
  if (pc) {
    $('.mCSB_container').children().mCustomScrollbar("update");
  }
}

function draggableDIV() {
  if ($('#change_width').length > 0 && $('#B').length > 0) {
    var awdata = "calc(100% - " + $('[data-changeWidth]').attr('data-changeWidth') + ")";
    var bwdata = $('[data-changeWidth]').attr('data-changeWidth');

    if ($('#A').attr('style') == undefined) {
      $('#A').css('width', awdata);
      $('#B').css('width', bwdata);
    }
    $('#Z').css('left', $('#B').offset().left + 15);
    var A = parseInt($('#A').width(), 10),
      W = parseInt($('#change_width').width(), 10),
      minw = 250,
      offset = $('#change_width').offset(),
      aw = parseInt($('#A').width(), 10);
    var phantramA = aw / (W / 100);
    var phantramB = 100 - phantramA;
    var phantramZ = ($('#Z').offset().left - 15) / ($('body').width() / 100);
    $('#A').css('width', phantramA + "%");
    $('#B').css('width', phantramB + "%");
    $('#Z').css('left', phantramZ + "%");

    splitter = function(event, ui) {
      A = parseInt($('#A').width(), 10);
      Z = parseInt($('#Z').width(), 10);
      W = parseInt($('#change_width').width(), 10);
      offset = $('#change_width').offset();
      aw = parseInt(ui.position.left) - offset.left;
      phantramA = aw / (W / 100);
      phantramB = 100 - phantramA;
      $('#A').css({
        width: phantramA + "%"
      });
      $('#B').css({
        width: phantramB + "%"
      });

    };
    $('#Z').draggable({
      axis: 'x',
      containment: [
        // offset.left + minw,
        offset.left + W / 2,
        offset.top,
        // offset.left + A + B - minw,
        offset.left + W - minw,
        offset.top + $('#change_width').height()
      ],
      drag: splitter,
      stop: function() {
        $('#Z').css('left', (($('#A').offset().left + parseInt($('#A').width(), 10)) / (parseInt($('body').width(), 10) / 100)) + "%");
        scrollupdate();
        $('#B').css({
          display: ""
        });
      }
    });
    // ẩn cột phải khi rỗng
    if ($.trim($("#B .content-change_width").html()) == "" || $("#B .content-change_width").length < 1) {
      $('#A').css({
        width: "100%"
      });
      if ($("#B .content-change_width").length > 0) {
        $('#B').css({
          width: "0%",
          display: "none"
        });
        $('#Z').addClass('fix-z');
        $('#Z').css({
          left: "100%"
        });
      }
    }
    var touchtime = 0;
    if (mobile || ipad || ipadNgang) {
      $('#Z').on("click", function() {
        if (touchtime == 0) {
          // set first click
          touchtime = new Date().getTime();
        } else {
          // compare first click to this click and see if they occurred within double click threshold
          if (((new Date().getTime()) - touchtime) < 300) {
            // double click occurred
            if ($('#B').width() >= 100) {
              $('#A').css({
                width: "100%"
              });
              $('#B').css({
                width: "0%",
                display: "none"
              });
              $('#Z').addClass('fix-z');
              $('#Z').css({
                left: "100%"
              });
            } else {
              $('#B').css('display', "");
              $('#A').css('width', awdata);
              $('#B').css('width', bwdata);
              $('#Z').css('left', awdata);
              $('#Z').removeClass('fix-z');
              A = parseInt($('#A').width(), 10);
              W = parseInt($('#change_width').width(), 10);

              offset = $('#change_width').offset();
              aw = parseInt($('#A').width(), 10);
              // bw =  A + B - aw;
              $('#A').css('width', A);
              $('#B').css('width', B);
              $('#Z').css('left', A);
              phantramA = aw / (W / 100);
              phantramB = 100 - phantramA;
              $('#A').css('width', phantramA + "%");
              $('#B').css('width', phantramB + "%");
              $('#Z').css('left', phantramA + "%");
            };
            scrollupdate();
            touchtime = 0;
          } else {
            // not a double click so set as a new first click
            touchtime = new Date().getTime();
          }
        }
      });
    }
    if (pc) {
      $('#Z').dblclick(function() {
        if ($('#B').width() >= 100) {
          $('#A').css({
            width: "100%"
          });
          $('#B').css({
            width: "0%",
            display: "none"
          });

          $('#Z').css({
            left: "100%"
          });
          $('#Z').addClass('fix-z');
        } else {
          $('#B').css('display', "");
          $('#A').css('width', awdata);
          $('#B').css('width', bwdata);
          $('#Z').css('left', awdata);
          $('#Z').removeClass('fix-z');
          A = parseInt($('#A').width(), 10);
          W = parseInt($('#change_width').width(), 10);

          offset = $('#change_width').offset();
          aw = parseInt($('#A').width(), 10);
          // bw =  A + B - aw;
          $('#A').css('width', A);
          $('#B').css('width', B);
          $('#Z').css('left', A);
          phantramA = aw / (W / 100);
          phantramB = 100 - phantramA;
          $('#A').css('width', phantramA + "%");
          $('#B').css('width', phantramB + "%");
          $('#Z').css('left', phantramA + "%");
        };
        scrollupdate();
      });
    }
    $('.button-collapse').click(function() {
      setTimeout(function() {
        var zz = $('#A').offset().left + parseInt($('#A').width(), 10);
        zz = zz / (parseInt($('body').width(), 10) / 100);
        $('#Z').css('left', zz + "%");
        scrollupdate();
      }, 200);
    });
  } else {
    $('#A').css('width', "100%");
  }
}






functionTreetinhvan();
$('.table tbody tr').hover(function() {
  if ($(this).find('.icon-list').length > 0) {
    var iconlist = $(this).find('.icon-list');
    iconlist.css('width', '');
    if (iconlist.offset().left < iconlist.closest('td').offset().left) {
      iconlist.addClass('icon-list-fix');
      // .css('width', $(this).closest('.scroll-table').width())
    } else {
      iconlist.removeClass('icon-list-fix');
      iconlist.css('width', '');
    }
  }
});

$('.side-nav.side-nav-light .collapsible-body ul li > a').click(function() {
  if (!$(this).closest('li').find('ul').hasClass('show')) {
    $(this).closest('li').find('ul').slideDown().addClass('show');
    $(this).closest('li').siblings().find('ul').slideUp().removeClass('show');
    $(this).addClass('active');
    $(this).closest('li').siblings().find('a').removeClass('active');
  } else {
    $(this).closest('li').find('ul').slideUp().removeClass('show');
    $(this).removeClass('active');
  }
});
// $('#slide-out .button-collapse, .button_side-nav .button-collapse').click(function(){
//   $('.menucap3').animate({left: '-240'});
// });
$('tbody tr').click(function() {
  $(this).addClass('selected');
  $(this).siblings().removeClass('selected');
})
$('td .form-control').on('focus', function() {
  $(this).parent().addClass('focus');
  // $(this).closest('tr').addClass('selected');
}).blur(function() {
  $(this).parent().removeClass('focus');
});
$.fn.textWidth = function(text, font) {
  if (!$.fn.textWidth.fakeEl) $.fn.textWidth.fakeEl = $('<span>').hide().appendTo(document.body);
  $.fn.textWidth.fakeEl.text(text || this.val() || this.text() || this.attr('placeholder')).css('font', font || this.css('font'));
  return $.fn.textWidth.fakeEl.width();
};
$('td input[type="text"]').each(function() {
  $(this).attr('title', $(this).val());
  $(this).attr('data-toggle', "tooltip");
  $(this).on('input.form-control', function() {
    $(this).attr('title', $(this).val());
    $(this).attr('data-original-title', $(this).val());
    var inputWidth = $(this).textWidth();
    $(this).css({
      width: inputWidth
    })
  }).trigger('input');
});
$("body").tooltip({
  selector: '[data-toggle=tooltip]'
});

// $('#Z[data-toggle=tooltip]').hover(function (e) {
//   setTimeout(function(){$('[role="tooltip"]').css({top: '-50%', "margin-top":'65px'});}, 100);
// });

function inputWidth(elem, minW, maxW) {
  elem = $(this);
}

var targetElem = $('.width-dynamic');

inputWidth(targetElem);

// $.fn.datepicker.dates['vi'] = {
//   days: ["Chủ nhật", "Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sau", "Thứ bảy"],
//   daysShort: ["CN", "T2", "T3", "T4", "T5", "T6", "T7"],
//   daysMin: ["CN", "T2", "T3", "T4", "T5", "T6", "T7"],
//   months: ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"],
//   monthsShort: ["Th1", "Th2", "Th3", "Th4", "Th5", "Th6", "Th7", "Th8", "Th9", "Th10", "Th11", "Th12"],
//   today: "Today",
//   clear: "Clear",
//   format: "mm/dd/yyyy",
//   titleFormat: "MM yyyy",
//   /* Leverages same syntax as 'format' */
//   weekStart: 0
// };
//
//
// $('.date').each(function(){
//   if($(this).find('.form-control:not([readonly])').length > 0){
//     $(this).datepicker({
//       autoclose: true,
//       todayHighlight: true,
//       language: 'vi'
//     });
//   }
// });

// .on('show', function(e) {
//       // $('.datepicker-orient-top').css('top',$(this).offset().top);
//     });
$('select.form-control:not([multiple])').select2();
$('select.form-control[multiple]').select2({
  closeOnSelect: false,
});
// $('select.form-control').each(function() {
//
//   var placeholder = 'Chọn';
//   var select2Class = $(this).attr('class');
//   if($(this).attr('placeholder') != undefined){
//     placeholder = $(this).attr('placeholder');
//   }
//   $(this).attr({'data-placeholder': placeholder, "data-allow-clear":"true"});
//   if($(this).attr('id') != undefined){
//     if ($(this).is('[multiple]')) {
//       $("#"+$(this).attr('id')).select2({
//         closeOnSelect: false,
//       });
//     } else {
//         $("#"+$(this).attr('id')).select2({
//       });
//     }
//   } else {
//
//   }
//   if ($(this).is('[multiple]')) {
//     $(this).select2({
//       closeOnSelect: false,
//     });
//   } else {
//     $(this).select2();
//   }
// });
var scrollY = 300;

var table = $('#example').dataTable({
  scrollY: scrollY,
  scrollX: true,
  scrollCollapse: true,
  paging: false,
  ordering: false,
  info: false,
  fixedColumns: {
    leftColumns: 1,
    rightColumns: 1
  },
  "initComplete": function(settings, json) {
    $(this).find("tr").hover(function() {
      var indextr = $(this).index() + 2;
      var closestTable = $(this).closest('.dataTables_wrapper');
      closestTable.find(".DTFC_LeftWrapper tr").removeClass('hover').eq(indextr).addClass('hover');
      closestTable.find(".DTFC_RightWrapper tr").removeClass('hover').eq(indextr).addClass('hover');
    }).click(function() {
      var indextr = $(this).index() + 2;
      var closestTable = $(this).closest('.dataTables_wrapper');
      closestTable.find(".DTFC_LeftWrapper tr").removeClass('selected').eq(indextr).addClass('selected');
      closestTable.find(".DTFC_RightWrapper tr").removeClass('selected').eq(indextr).addClass('selected');
    })
  }
});

$('.footer .btn').click(function() {
  $(this).addClass('active').siblings().removeClass('active');
})



$('th input[type="checkbox"]').change(function() {
  var name_th_input = 'td [name="' + $(this).attr('name') + '"]';
  if (this.checked) {
    $(this).prop("checked", true);
    $(name_th_input).prop("checked", true);
  } else {
    $(this).prop("checked", false);
    $(name_th_input).prop("checked", false);
  }
});
$('td input[type="checkbox"]').change(function() {
  var name_td_inputCk = 'td [name="' + $(this).attr('name') + '"]:checked';
  var name_td_input = 'td [name="' + $(this).attr('name') + '"]';
  var name_th_input = 'th [name="' + $(this).attr('name') + '"]';
  if ($(name_td_inputCk).length == $(name_td_input).length) {
    $(name_th_input).prop("checked", true);
  } else {
    $(name_th_input).prop("checked", false);
  }
});

/* Sample function that returns boolean in case the browser is Internet Explorer*/
function isIE() {
  ua = navigator.userAgent;
  /* MSIE used to detect old browsers and Trident used to newer ones*/
  var is_ie = ua.indexOf("MSIE ") > -1 || ua.indexOf("Trident/") > -1;

  return is_ie;
}

$(document).ready(function() {
  /* Create an alert to show if the browser is IE or not */
  if (isIE()) {
    $("#slide-out,#slide-out .collapsible-body, .menucap3 .ul").mCustomScrollbar({
      theme: "minimal-dark",
      axis: "y",
      scrollbarPosition: "outside",
    });

    $('.navbar.scrolling-navbar').css('z-index', '9999');
    $('.button-collapse').click(function() {
      $('#slide-out').toggle();
      $('.noidungchinh').toggleClass('full');
      $('.footer').toggleClass('full');

    });

    $('.collapsible a').click(function() {
      if (!$(this).closest('li').find('.collapsible-body').hasClass('show')) {
        $(this).closest('li').find('.collapsible-body').slideDown().addClass('show');
        $(this).closest('li').siblings().find('.collapsible-body').slideUp().removeClass('show');
        $(this).addClass('active');
        $(this).closest('li').siblings().find('a').removeClass('active');
      } else {
        $(this).closest('li').find('.collapsible-body').slideUp().removeClass('show');
        $(this).removeClass('active');
      }
    });
  } else {
      $("#slide-out,#slide-out .collapsible-body, .menucap3 .ul").mCustomScrollbar({
          theme: "minimal-dark",
          axis: "y",
          scrollbarPosition: "outside",
      });

      $('.navbar.scrolling-navbar').css('z-index', '9999');
      $('.button-collapse').click(function () {
          $('#slide-out').toggle();
          $('.noidungchinh').toggleClass('full');
          $('.footer').toggleClass('full');

      });

      $('.collapsible a').click(function () {
          if (!$(this).closest('li').find('.collapsible-body').hasClass('show')) {
              $(this).closest('li').find('.collapsible-body').slideDown().addClass('show');
              $(this).closest('li').siblings().find('.collapsible-body').slideUp().removeClass('show');
              $(this).addClass('active');
              $(this).closest('li').siblings().find('a').removeClass('active');
          } else {
              $(this).closest('li').find('.collapsible-body').slideUp().removeClass('show');
              $(this).removeClass('active');
          }
      });
      // document.write('<script src="js/mdb.min.js"><\/script>');
    //sideNavCuttom();
    // if(pc){
    //   $('body').mCustomScrollbar({
    //     theme: "minimal-dark",
    //     axis: "yx",
    //   });
    // }
    $(".table").each(function() {
      if (!$(this).parent().hasClass('scroll-table') && !$(this).hasClass('no-scroll')) {
        $(this).wrap('<div class="scroll-table"></div>');
        $(this).find('thead th').each(function() {
          $(this).attr('width', $(this).width());
        });
        $(this).find('thead th:last-child').css('width', 'auto');
        if (pc) {
          $(this).parent('.scroll-table').mCustomScrollbar({
            theme: "minimal-dark",
            axis: "yx",
            scrollbarPosition: "outside",
            scrollInertia: 0,
          });
        }
      }
    });
    // $('td.dropdown').on('show.bs.dropdown', function() {
    //   $(this).closest('.scroll-table').addClass('dropdown-menu-show');
    // });
    // $('td.dropdown').on('hide.bs.dropdown', function() {
    //   $(this).closest('.scroll-table').removeClass('dropdown-menu-show');
    // });
  }

  draggableDIV();

  // $('body').on('click', '.dropdown .form-check.dropdown-item, .search-dropdown .dropdown-menu, .select2-container',function(e) {
  //   return false;
  // });
  $('body').click(function(e) {
    if ($(e.target).closest('.show').length < 1) {
      if ($(e.target).closest('.dropdown').length < 1) {
        $('.dropdown.show').find('[data-toggle="dropdown"]').click();
      }
    } else {
      return false;
    }
  });

  ////$('.mdb-select').materialSelect();

  $('.button-collapse').click(function() {
    if ($(this).parent().hasClass('button_side-nav')) {
      $('.noidungchinh').removeClass('full');
      $('.footer').removeClass('full');
      $('#slide-out').css('display', 'block');
    } else {
      $('.noidungchinh').addClass('full');
      $('.footer').addClass('full');
      $('#slide-out').css('display', '');
    }
  });

});
$('.modal').on('shown.bs.modal', function() {
  scrollupdate();
})
var a = 1;
$(window).resize(function() {
  // testmobile();
  function fixSscrollY() {
    if (window.scrollY > window.innerHeight && a == 1) {
      draggableDIV();
      a = 0;
    }
    if (window.scrollY <= window.innerHeight) {
      a = 1;
    }
  }
  fixSscrollY();
  // if(mobile || ipad){
  //   $('.mCSB_container').children().mCustomScrollbar("destroy");
  //   $('body').mCustomScrollbar("destroy");
  // }
});
$.fn.mobileDropdown = function() {
  var buttonDropdown = '<button type="button" class="cuttom-collapse-btn btn btn-link btn-sm waves-effect waves-light float-right"><i class="fa fa-angle-down" aria-hidden="true"></i></button>',
    divDropdown = '<div class="cuttom-collapse"></div>',
    thisConten = this;
  this.wrap(divDropdown);
  this.before(buttonDropdown);
  thisConten.hide();
  this.parent().find('.cuttom-collapse-btn').click(function() {
    thisConten.slideToggle();
    thisConten.parent().toggleClass('show');
  })
};
if (mobile || ipad) {
  $('ul.tabvertical').mobileDropdown();
}
var ul = $('#upload ul');
$('#drop a').click(function() {
  // Simulate a click on the file input button
  // to show the file browser dialog
  $(this).parent().find('input').click();
});
////// Initialize the jQuery File Upload plugin
////$('#upload').fileupload({
////  // This element will accept file drag/drop uploading
////  dropZone: $('#drop'),
////  // This function is called when a file is added to the queue;
////  // either via the browse button, or via drag/drop:
////  add: function(e, data) {
////    var tpl = $('<li class="working"><i class="fa fa-paperclip" aria-hidden="true"></i><p></p><span></span><div class="form-group row"><input type="text" class="form-control" placeholder="Mô tả"><div></li>');
////    // Append the file name and file size
////    tpl.find('p').text(data.files[0].name)
////      .append('<i>' + formatFileSize(data.files[0].size) + '</i>');
////    // Add the HTML to the UL element
////    data.context = tpl.appendTo(ul);
////    // Initialize the knob plugin
////    // tpl.find('input').knob();
////    // Listen for clicks on the cancel icon
////    tpl.find('span').click(function() {
////      if (tpl.hasClass('working')) {
////        jqXHR.abort();
////      }
////      tpl.fadeOut(function() {
////        tpl.remove();
////      });
////    });
////    // Automatically upload the file once it is added to the queue
////    var jqXHR = data.submit();
////  },
////  progress: function(e, data) {
////    // Calculate the completion percentage of the upload
////    var progress = parseInt(data.loaded / data.total * 100, 10);
////    // Update the hidden input field and trigger a change
////    // so that the jQuery knob plugin knows to update the dial
////    data.context.find('input').val(progress).change();
////    if (progress == 100) {
////      data.context.removeClass('working');
////    }
////  },
////  fail: function(e, data) {
////    // Something has gone wrong!
////    data.context.addClass('error');
////  }
////});


// Prevent the default action when a file is dropped on the window
$(document).on('drop dragover', function(e) {
  e.preventDefault();
});

// Helper function that formats the file sizes
function formatFileSize(bytes) {
  if (typeof bytes !== 'number') {
    return '';
  }

  if (bytes >= 1000000000) {
    return (bytes / 1000000000).toFixed(2) + ' GB';
  }

  if (bytes >= 1000000) {
    return (bytes / 1000000).toFixed(2) + ' MB';
  }

  return (bytes / 1000).toFixed(2) + ' KB';
}


$("#sel_2").select2ToTree({
  closeOnSelect: false
});
$("#sel_3").select2ToTree();

Array.prototype.remove = function() {
  var what, a = arguments,
    L = a.length,
    ax;
  while (L && this.length) {
    what = a[--L];
    while ((ax = this.indexOf(what)) !== -1) {
      this.splice(ax, 1);
    }
  }
  return this;
};

var valop = [];
$('.tree-search').on('select2:select', function(e) {
  var thisSelect2 = $(this),
    data = e.params.data,
    dataOP = '.' + $(data.element).attr("data-id"),
    valueOP = $(data.element).attr("value");
  valop.push(valueOP);
  $(dataOP, thisSelect2).each(function() {
    valop.push($(this).val());
  });
  thisSelect2.val(valop);
  thisSelect2.trigger('change');
});
$('.tree-search').on('select2:unselect', function(e) {
  var thisSelect2 = $(this),
    data = e.params.data,
    dataOP = '.' + $(data.element).attr("data-id"),
    valueOP = $(data.element).attr("value");
  valop.remove(valueOP);
  $(dataOP, thisSelect2).each(function() {
    valop.remove($(this).val());
  });
  thisSelect2.val(valop);
  thisSelect2.trigger('change');
});
// // chọn nhiều cột
// var table = $("#example2");
//
// var isMouseDown = false;
// var startRowIndex = null;
// var startCellIndex = null;
//
// function selectTo(cell) {
//
//     var row = cell.parent();
//     var cellIndex = cell.index();
//     var rowIndex = row.index();
//
//     var rowStart, rowEnd, cellStart, cellEnd;
//
//     if (rowIndex < startRowIndex) {
//         rowStart = rowIndex;
//         rowEnd = startRowIndex;
//     } else {
//         rowStart = startRowIndex;
//         rowEnd = rowIndex;
//     }
//
//     if (cellIndex < startCellIndex) {
//         cellStart = cellIndex;
//         cellEnd = startCellIndex;
//     } else {
//         cellStart = startCellIndex;
//         cellEnd = cellIndex;
//     }
//
//     for (var i = rowStart; i <= rowEnd+1; i++) {
//         var rowCells = table.find("tr").eq(i).find("td");
//         for (var j = cellStart; j <= cellEnd; j++) {
//             rowCells.eq(j).addClass("selected-td");
//         }
//     }
// }
//
// table.find("td").mousedown(function (e) {
//     isMouseDown = true;
//     var cell = $(this);
//     table.find(".selected-td").removeClass("selected-td"); // deselect everything
//     if (e.shiftKey) {
//         selectTo(cell);
//     } else {
//         cell.addClass("selected-td");
//         startCellIndex = cell.index();
//         startRowIndex = cell.parent().index();
//     }
//
//     return false; // prevent text selection
// })
// .mouseover(function () {
//     if (!isMouseDown) return;
//     table.find(".selected-td").removeClass("selected-td");
//     selectTo($(this));
// })
// .bind("selectstart", function () {
//     return false;
// });
//
// $(document).mouseup(function () {
//     isMouseDown = false;
// });
// $('td.dropdown > a').on('click', function(e) {
//   e.preventDefault();
// });
$('.cuttom-dropdown-a').click(function(e){
    e.preventDefault();
    $('ul.cuttom-dropdown').addClass('show').css({
    'top': $(this).offset().top + $(this).height(), 'right': $('body').width() - ($(this).offset().left + $(this).width()/3)
    });
    $('ul.cuttom-dropdown').attr('data-dropdowId',$(this).attr('id'));
    console.log($('ul.cuttom-dropdown').attr('data-dropdowId'));
});
$('body').click(function(e) {
  if ($(e.target).closest('.cuttom-dropdown.show').length < 1) {
    if ($(e.target).closest('.dropdown').length < 1) {
      $('ul.cuttom-dropdown').removeClass('show');
    }
  }
});
// $('[data-toggle="popover"]').popover({
//   html: true,
//   delay: { "hide": 300 }
// });
// $("[data-toggle=popover]")
//   .popover({
//     html: true
//   })
//   .on("focus", function() {
//     $(this).popover("show");
//   }).on("focusout", function() {
//     var _this = this;
//     if (!$(".popover:hover").length) {
//       $(this).popover("hide");
//     } else {
//       $('.popover').mouseleave(function() {
//         $(_this).popover("hide");
//         $(this).off('mouseleave');
//       });
//     }
//   });
// $("[data-toggle='popover']").on('shown.bs.popover', function() {
//   $('.popover-header').addClass($(this).closest('td').attr('class'));
// });
