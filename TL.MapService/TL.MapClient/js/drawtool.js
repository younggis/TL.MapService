/**
 * OpenLayers 3 DrawTool Control.
 * See [the examples](./examples) for usage.
 * @constructor
 * @extends {ol.control.Control}
 * @param {Object} opt_options Control options, extends olx.control.ControlOptions adding:
 *                              **`tipLabel`** `String` - the button tooltip.
 */
ol.control.DrawTool = function (opt_options) {

    var options = opt_options || {};

    this.sphereradius = options.sphereradius ?
      options.sphereradius : 6378137;

    this.mapListeners = [];

    // hiddenclass
    this.hiddenClassName = 'ol-control DrawTool';
    if (ol.control.DrawTool.isTouchDevice_()) {
        this.hiddenClassName += ' touch';
    }
    // shownClass
    this.shownClassName = this.hiddenClassName + ' shown';

    var element = document.createElement('div');
    element.className = this.hiddenClassName;

    this.panel = document.createElement('ul');
    element.appendChild(this.panel);

    var ulheader = document.createElement('li');
    this.panel.appendChild(ulheader);

    var inputMeasure = document.createElement('input');
    inputMeasure.type = "button";
    ulheader.appendChild(inputMeasure);

    var ulbody = document.createElement('li');
    this.panel.appendChild(ulbody);

    var html = '';
    html += '<ul class="ulbody">';
    html += '<li><input type="button" value="LineString"></li>';
    html += '<li><input type="button" value="Circle"></li>';
    html += '<li><input type="button" value="Polygon"></li>';
    html += '<li><input type="checkbox" value="no"></li>';
    html += '</ul>';
    ulbody.innerHTML = html;

    var this_ = this;

    inputMeasure.onmouseover = function (e) {
        this_.showPanel();
    };
    inputMeasure.onclick = function (e) {
        e = e || window.event;
        this_.showPanel();
        e.preventDefault();
    };

    var lis = ulbody.getElementsByTagName("li");

    this.source = new ol.source.Vector();
    this.vector = new ol.layer.Vector({
        source: this.source,
        style: new ol.style.Style({
            fill: new ol.style.Fill({
                color: 'rgba(255, 255, 255, 0.2)'
            }),
            stroke: new ol.style.Stroke({
                color: '#ffcc33',
                width: 2
            }),
            image: new ol.style.Circle({
                radius: 7,
                fill: new ol.style.Fill({
                    color: '#ffcc33'
                })
            })
        })
    });

    //type length or area
    var typeSelect = {};
    //Line start
    lis[0].onclick = function (e) {
        typeSelect.value = 'LineString';
        typeSelect.check = lis[2].getElementsByTagName("input")[0].checked;
        this_.mapmeasure(typeSelect);
    };
    //Area start
    lis[1].onclick = function (e) {
        typeSelect.value = 'Circle';
        typeSelect.check = lis[2].getElementsByTagName("input")[0].checked;
        this_.mapmeasure(typeSelect);
    };
    lis[2].onclick = function (e) {
        typeSelect.value = 'Polygon';
        typeSelect.check = lis[2].getElementsByTagName("input")[0].checked;
        this_.mapmeasure(typeSelect);
    };
    $('.draw-btn').on('click', function () {
        var _this = $(this);
        $('.draw-btn').siblings().removeClass('active');
        _this.addClass('active');
    });
    $('#drawline').on('click', function () {
        typeSelect.value = 'LineString';
        typeSelect.check = true;
        this_.mapmeasure(typeSelect);
    });
    $('#drawcircle').on('click', function () {
        typeSelect.value = 'Circle';
        typeSelect.check = true;
        this_.mapmeasure(typeSelect);
    });
    $('#drawpolygon').on('click', function () {
        typeSelect.value = 'Polygon';
        typeSelect.check = true;
        this_.mapmeasure(typeSelect);
    });

    this_.panel.onmouseout = function (e) {
        e = e || window.event;
        if (!this_.panel.contains(e.toElement || e.relatedTarget)) {
            this_.hidePanel();
        }
    };

    ol.control.Control.call(this, {
        element: element,
    });

};

ol.inherits(ol.control.DrawTool, ol.control.Control);

ol.control.DrawTool.prototype.mapmeasure = function (typeSelect) {
    var source = this.source;
    var vector = this.vector;
    var wgs84Sphere = new ol.Sphere(this.sphereradius);

    var sketch;
    var helpTooltipElement;
    var DrawTooltipElement;
    var DrawTooltip;

    var map = this.getMap();
    map.addLayer(vector);

    map.getViewport().addEventListener('mouseout', function () {
        helpTooltipElement.classList.add('hidden');
    });

    var draw; // global so we can remove it later

    var formatLength = function (line) {
        var length;
        if (typeSelect.check) {
            var coordinates = line.getCoordinates();
            length = 0;
            var sourceProj = map.getView().getProjection();
            for (var i = 0, ii = coordinates.length - 1; i < ii; ++i) {
                var c1 = ol.proj.transform(coordinates[i], sourceProj, 'EPSG:4326');
                var c2 = ol.proj.transform(coordinates[i + 1], sourceProj, 'EPSG:4326');
                length += wgs84Sphere.haversineDistance(c1, c2);
            }
        } else {
            var sourceProj = map.getView().getProjection();
            var geom = /** @type {ol.geom.Polygon} */(line.clone().transform(
                sourceProj, 'EPSG:3857'));
            length = Math.round(geom.getLength() * 100) / 100;
            // length = Math.round(line.getLength() * 100) / 100;
        }
        var output;
        if (length > 100) {
            output = (Math.round(length / 1000 * 100) / 100) +
                ' ' + 'km';
        } else {
            output = (Math.round(length * 100) / 100) +
                ' ' + 'm';
        }
        return output;
    };

    var formatArea = function (polygon) {
        if (typeSelect.check) {
            var sourceProj = map.getView().getProjection();
            var geom = /** @type {ol.geom.Polygon} */(polygon.clone().transform(
                sourceProj, 'EPSG:4326'));
            var coordinates = geom.getLinearRing(0).getCoordinates();
            area = Math.abs(wgs84Sphere.geodesicArea(coordinates));
        } else {
            var sourceProj = map.getView().getProjection();
            var geom = /** @type {ol.geom.Polygon} */(polygon.clone().transform(
                sourceProj, 'EPSG:3857'));
            area = geom.getArea();
            // area = polygon.getArea();
        }
        var output;
        if (area > 10000) {
            output = (Math.round(area / 1000000 * 100) / 100) +
                ' ' + 'km<sup>2</sup>';
        } else {
            output = (Math.round(area * 100) / 100) +
                ' ' + 'm<sup>2</sup>';
        }
        return output;
    };

    var popupcloser = document.createElement('a');
    popupcloser.href = 'javascript:void(0);';
    popupcloser.classList.add('ol-popup-closer');

    function addInteraction() {
        var type = typeSelect.value;
        draw = new ol.interaction.Draw({
            source: source,
            type: /** @type {ol.geom.GeometryType} */ (type),
            style: new ol.style.Style({
                fill: new ol.style.Fill({
                    color: 'rgba(255, 255, 255, 0.2)'
                }),
                stroke: new ol.style.Stroke({
                    color: 'rgba(0, 0, 0, 0.5)',
                    lineDash: [10, 10],
                    width: 2
                }),
                image: new ol.style.Circle({
                    radius: 5,
                    stroke: new ol.style.Stroke({
                        color: 'rgba(0, 0, 0, 0.7)'
                    }),
                    fill: new ol.style.Fill({
                        color: 'rgba(255, 255, 255, 0.2)'
                    })
                })
            })
        });
        map.addInteraction(draw);

        createDrawTooltip();
        createHelpTooltip();

        var listener;
        draw.on('drawstart',
          function (evt) {

              vector.getSource().clear();
              // set sketch
              sketch = evt.feature;

              /** @type {ol.Coordinate|undefined} */
              var tooltipCoord = evt.coordinate;

              listener = sketch.getGeometry().on('change', function (evt) {
                  try {
                      var geom = evt.target;
                      var output;
                      if (geom instanceof ol.geom.Polygon) {
                          output = formatArea(geom);
                          tooltipCoord = geom.getInteriorPoint().getCoordinates();
                      } else if (geom instanceof ol.geom.LineString) {
                          output = formatLength(geom);
                          tooltipCoord = geom.getLastCoordinate();
                      }
                      DrawTooltipElement.innerHTML = output;
                      DrawTooltip.setPosition(tooltipCoord);
                  } catch (e) {
                      map.removeInteraction(draw);
                  } finally {
                  }

              });
          }, this);

        draw.on('drawend',
            function () {
                DrawTooltipElement.appendChild(popupcloser);
                DrawTooltipElement.className = 'tooltip tooltip-static';
                DrawTooltip.setOffset([0, -7]);
                // unset sketch
                sketch = null;
                // unset tooltip so that a new one can be created
                DrawTooltipElement = null;
                //createDrawTooltip();
                ol.Observable.unByKey(listener);
                //end
                map.removeInteraction(draw);
                $('.draw-btn').siblings().removeClass('active');
                layer.msg('确定更新？', {
                    time: 0,
                    btn: ['确定', '取消'],
                    yes:function(index) {
                        layer.close(index);
                        updateMap();
                    }
            });
                // map.getInteractions().item(1).setActive(false);
            }, this);
    }
    function updateMap() {
        var extent = vector.getSource().getExtent();
        if (Math.abs((extent[2] - extent[0]) * (extent[1] - extent[3])) >= 10000000000) {
            layer.msg("范围太大，请选择更小范围", { icon: 0 });
            return;
        }
        var loadlayer = layer.load(1, { shade: [0.8, '#393D49'] });
        $.getJSON("http://localhost:9152/MapService.svc/UpdateMap?jsoncallback=?&maptype=tdt-road&minx="
            + extent[0] + "&maxy="
            + extent[1] + "&maxx="
            + extent[2] + "&miny="
            + extent[3]
            , function (result) {
                layer.close(loadlayer);
                if (result == true || result == "true") {
                    layer.msg("更新成功，请重新加载页面", { icon: 1 });
                } else {
                    layer.msg("更新失败，请重新尝试更新", { icon: 2 });
                }
            },function(e) {
                layer.close(loadlayer);
                layer.msg("更新失败，请重新尝试更新", { icon: 2 });
        });
    }

    function createHelpTooltip() {
        if (helpTooltipElement) {
            helpTooltipElement.parentNode.removeChild(helpTooltipElement);
        }
        helpTooltipElement = document.createElement('div');
        helpTooltipElement.className = 'tooltip hidden';
    }
    function createDrawTooltip() {
        if (DrawTooltipElement) {
            DrawTooltipElement.parentNode.removeChild(DrawTooltipElement);
        }
        DrawTooltipElement = document.createElement('div');
        DrawTooltipElement.className = 'tooltip tooltip-draw';
        DrawTooltip = new ol.Overlay({
            //element: DrawTooltipElement,
            offset: [0, -15],
            positioning: 'bottom-center'
        });
        map.addOverlay(DrawTooltip);

    }

    //clear
    popupcloser.onclick = function (e) {
        //map.getOverlays().clear();
        $(this).parent().parent().remove();//防止影响地图的覆盖物图层
        vector.getSource().clear();
        // map.removeLayer(vector);
    };

    addInteraction();
};

/**
 * Show the DrawTool.
 */
ol.control.DrawTool.prototype.showPanel = function () {
    if (this.element.className != this.shownClassName) {
        this.element.className = this.shownClassName;
    }
};

/**
 * Hide the DrawTool.
 */
ol.control.DrawTool.prototype.hidePanel = function () {
    if (this.element.className != this.hiddenClassName) {
        this.element.className = this.hiddenClassName;
    }
};

/**
 * Set the map instance the control is associated with.
 * @param {ol.Map} map The map instance.
 */
ol.control.DrawTool.prototype.setMap = function (map) {
    // Clean up listeners associated with the previous map
    for (var i = 0, key; i < this.mapListeners.length; i++) {
        this.getMap().unByKey(this.mapListeners[i]);
    }
    this.mapListeners.length = 0;
    // Wire up listeners etc. and store reference to new map
    ol.control.Control.prototype.setMap.call(this, map);
    if (map) {
        var this_ = this;
        this.mapListeners.push(map.on('pointerdown', function () {
            this_.hidePanel();
        }));
    }
};

/**
 * Generate a UUID
 * @returns {String} UUID
 *
 * Adapted from http://stackoverflow.com/a/2117523/526860
 */
ol.control.DrawTool.uuid = function () {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

/**
* @private
* @desc Apply workaround to enable scrolling of overflowing content within an
* element. Adapted from https://gist.github.com/chrismbarr/4107472
*/
ol.control.DrawTool.enableTouchScroll_ = function (elm) {
    if (ol.control.DrawTool.isTouchDevice_()) {
        var scrollStartPos = 0;
        elm.addEventListener("touchstart", function (event) {
            scrollStartPos = this.scrollTop + event.touches[0].pageY;
        }, false);
        elm.addEventListener("touchmove", function (event) {
            this.scrollTop = scrollStartPos - event.touches[0].pageY;
        }, false);
    }
};

/**
 * @private
 * @desc Determine if the current browser supports touch events. Adapted from
 * https://gist.github.com/chrismbarr/4107472
 */
ol.control.DrawTool.isTouchDevice_ = function () {
    try {
        document.createEvent("TouchEvent");
        return true;
    } catch (e) {
        return false;
    }
};
