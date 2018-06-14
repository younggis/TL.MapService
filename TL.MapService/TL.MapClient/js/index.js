var map, view, measureTool, drawTool;//地图、视图、弹出层
var scene;
var maptype;
function init() {
    var projection = ol.proj.get('EPSG:3857');
    var minZoom = 2; //地图最小等级
    var maxZoom = 20; //地图最大等级
    var centX = 11577545; //初始化地图中心点x
    var centY = 3579865; //初始化地图中心点y

    // 初始化显示视图
    view = new ol.View({
        center: [centX, centY],
        projection: projection,
        zoom: 4,
        minZoom: minZoom,
        maxZoom: maxZoom
    });
    // 初始化地图
    map = new ol.Map({
        controls: ol.control.defaults().extend([
            new ol.control.OverviewMap(), //鹰眼控件
            new ol.control.ScaleLine(), //比例尺控件
            new ol.control.MousePosition({
                //实时获取鼠标位置信息
                coordinateFormat: ol.coordinate.createStringXY(6),
                projection: ol.proj.get('EPSG:4326'),
                className: 'custom-mouse-position',
                target: document.getElementById('mouse-position')
            })
        ]),
        layers: [],
        overlays: [], //覆盖层
        loadTilesWhileAnimating: true,
        target: 'map',
        view: view
    });
    measureTool = new ol.control.MeasureTool({
        sphereradius: 6378137
    });
    map.addControl(measureTool);

    drawTool = new ol.control.DrawTool({
        sphereradius: 6378137
    });
    map.addControl(drawTool);

    new BaseLayer.init(map, '');//初始化底图对象

    $('.map-group .map-item').on('click', function () {
        $(this).siblings().removeClass('active');
        $(this).addClass('active');
        var type = $(this).attr('data-name');
        new BaseLayer.setBaseLayer(type);
    });
}
$(function () {
    init();
    $('#baselayer').on('click', function () {
        layer.open({
            type: 1,
            area: ['794px', '557px'], //宽高
            shade: false,
            title: '底图', //不显示标题
            content: $('#map-list') //捕获的元素，注意：最好该指定的元素要存放在body最外层，否则可能被其它的相对元素所影响
        });
    });
    $('#measure').on('click', function () {
        layer.open({
            type: 1,
            area: ['140px', '107px'], //宽高
            offset: ['480px', '100px'],
            shade: false,
            title: '测量', //不显示标题
            content: $('#measure-list'), //捕获的元素，注意：最好该指定的元素要存放在body最外层，否则可能被其它的相对元素所影响
            cancel: function () {
                try {
                    map.getOverlays().clear();
                    measureTool.vector.getSource().clear();
                } catch (e) {
                    console.log(e.message);
                }
            }
        });
    });
    $('#update').on('click', function () {
        layer.open({
            type: 1,
            area: ['174px', '107px'], //宽高
            offset: ['580px', '100px'],
            shade: false,
            title: '更新', //不显示标题
            content: $('#draw-list'), //捕获的元素，注意：最好该指定的元素要存放在body最外层，否则可能被其它的相对元素所影响
            cancel: function () {
                getExtent();
                try {
                    map.getOverlays().clear();
                    drawTool.vector.getSource().clear();
                } catch (e) {
                    console.log(e.message);
                }
            }
        });
    });
    $('#fullmap').on('click', function () {
        fullExtent();
    });
    $('#layergroup').on('click', function () {
        layer.open({
            type: 1,
            area: ['320px', '307px'], //宽高
            offset: ['60px', '100px'],
            shade: false,
            title: '图层', //不显示标题
            content: $('#layer-list') //捕获的元素，注意：最好该指定的元素要存放在body最外层，否则可能被其它的相对元素所影响
        });
    });
});

function getExtent() {
    var extent = drawTool.vector.getSource().getExtent();
    var zoom = map.getView().getZoom();
    var minx = getColRow(extent[0],extent[1],zoom);
    var maxy = getColRow(extent[2], extent[3], zoom);

    debugger;
}
//根据坐标计算行列号
function getColRow(x,y,z) {
    var resolution = [156543.03390625, 78271.516953125, 39135.7584765625, 19567.87923828125, 9783.939619140625, 4891.9698095703125, 2445.9849047851562, 1222.9924523925781, 611.4962261962891, 305.74811309814453, 152.87405654907226, 76.43702827453613, 38.218514137268066, 19.109257068634033, 9.554628534317017, 4.777314267158508, 2.388657133579254, 1.194328566789627, 0.5971642833948135, 0.29858214169740677, 0.14929107084870338, 0.07464553542435169, 0.037322767712175846, 0.018661383856087923, 0.009330691928043961, 0.004665345964021981, 0.0023326729820109904, 0.0011663364910054952, 5.831682455027476E-4, 2.915841227513738E-4, 1.457920613756869E-4];
    var origin = [-2.003750834E7, 2.003750834E7];
    var tileSize = 256;
    var col = Math.floor((origin[0] - x) / (tileSize * resolution[z]));
    var row = Math.floor((origin[1] - y) / (tileSize * resolution[z]));
    return {
        col: col,
        row:row
    }
}



//遍历数组，找到图层
function findlayer(layers, name) {
    for (var i = 0; i < layers.length; i++) {
        if (layers[i].layername == name) {
            return layers[i].tilelayer;
        }
    }
}

//全图
function fullExtent() {
    view.setCenter([11109563.7073, 3033643.034400001]);
    view.setZoom(5);
}

