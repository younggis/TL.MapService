var BaseLayer = {
    baiduroadlayer: null,
    baiduimagelayer: null,
    baidulabellayer: null,
    gaoderoadlayer: null,
    tiandituroadLayer: null,
    tianditulabelLayer: null,
    tiandituimgLayer: null,
    googleroadLayer: null,
    googleterainLayer: null,
    googleimgLayer: null,
    map: null,
    gridsetName: 'EPSG:3857',
    gridNames: ['EPSG:3857:0', 'EPSG:3857:1', 'EPSG:3857:2', 'EPSG:3857:3', 'EPSG:3857:4', 'EPSG:3857:5', 'EPSG:3857:6', 'EPSG:3857:7', 'EPSG:3857:8', 'EPSG:3857:9', 'EPSG:3857:10', 'EPSG:3857:11', 'EPSG:3857:12', 'EPSG:3857:13', 'EPSG:3857:14', 'EPSG:3857:15', 'EPSG:3857:16', 'EPSG:3857:17', 'EPSG:3857:18', 'EPSG:3857:19', 'EPSG:3857:20', 'EPSG:3857:21', 'EPSG:3857:22', 'EPSG:3857:23', 'EPSG:3857:24', 'EPSG:3857:25', 'EPSG:3857:26', 'EPSG:3857:27', 'EPSG:3857:28', 'EPSG:3857:29', 'EPSG:3857:30'],
    baseUrl: 'http://localhost:6090/geoserver/gwc/service/wmts',
    serverurl: 'http://localhost:9152/MapService.svc/GetMap?',
    style: '',
    format: 'image/png',
    infoFormat: 'text/html',
    projection: new ol.proj.Projection({
        code: 'EPSG:3857',
        units: 'm',
        axisOrientation: 'neu'
    }),
    resolutions: [156543.03390625, 78271.516953125, 39135.7584765625, 19567.87923828125, 9783.939619140625, 4891.9698095703125, 2445.9849047851562, 1222.9924523925781, 611.4962261962891, 305.74811309814453, 152.87405654907226, 76.43702827453613, 38.218514137268066, 19.109257068634033, 9.554628534317017, 4.777314267158508, 2.388657133579254, 1.194328566789627, 0.5971642833948135, 0.29858214169740677, 0.14929107084870338, 0.07464553542435169, 0.037322767712175846, 0.018661383856087923, 0.009330691928043961, 0.004665345964021981, 0.0023326729820109904, 0.0011663364910054952, 5.831682455027476E-4, 2.915841227513738E-4, 1.457920613756869E-4],
    baseParams: ['VERSION', 'LAYER', 'STYLE', 'TILEMATRIX', 'TILEMATRIXSET', 'SERVICE', 'FORMAT'],
    init: function (map, type) {
        BaseLayer.map = map;
        BaseLayer.baiduroadlayer = BaseLayer.BaiduRoad(); BaseLayer.map.addLayer(BaseLayer.baiduroadlayer); BaseLayer.baiduroadlayer.setVisible(false);
        BaseLayer.baiduimagelayer = BaseLayer.BaiduImage(); BaseLayer.map.addLayer(BaseLayer.baiduimagelayer); BaseLayer.baiduimagelayer.setVisible(false);
        BaseLayer.baidulabellayer = BaseLayer.BaiduLabel(); BaseLayer.map.addLayer(BaseLayer.baidulabellayer); BaseLayer.baidulabellayer.setVisible(false);

        BaseLayer.gaoderoadlayer = BaseLayer.GaodeRoad(); BaseLayer.map.addLayer(BaseLayer.gaoderoadlayer); BaseLayer.gaoderoadlayer.setVisible(false);
        BaseLayer.gaodeimagelayer = BaseLayer.GaodeImage(); BaseLayer.map.addLayer(BaseLayer.gaodeimagelayer); BaseLayer.gaodeimagelayer.setVisible(false);
        BaseLayer.gaodelabellayer = BaseLayer.GaodeLabel(); BaseLayer.map.addLayer(BaseLayer.gaodelabellayer); BaseLayer.gaodelabellayer.setVisible(false);

        BaseLayer.tiandituroadLayer = BaseLayer.TiandituRoadMap(); BaseLayer.map.addLayer(BaseLayer.tiandituroadLayer); BaseLayer.tiandituroadLayer.setVisible(false);
        BaseLayer.tiandituimgLayer = BaseLayer.TiandituImageMap(); BaseLayer.map.addLayer(BaseLayer.tiandituimgLayer); BaseLayer.tiandituimgLayer.setVisible(false);
        BaseLayer.tianditulabelLayer = BaseLayer.TiandituLabelMap(); BaseLayer.map.addLayer(BaseLayer.tianditulabelLayer); BaseLayer.tianditulabelLayer.setVisible(false);
        BaseLayer.googleroadLayer = BaseLayer.GoogleRoadMap(); BaseLayer.map.addLayer(BaseLayer.googleroadLayer); BaseLayer.googleroadLayer.setVisible(false);
        BaseLayer.googleimgLayer = BaseLayer.GoogleImageMap(); BaseLayer.map.addLayer(BaseLayer.googleimgLayer); BaseLayer.googleimgLayer.setVisible(false);
        BaseLayer.setBaseLayer(type);
    },
    setBaseLayer: function (type) {
        if (BaseLayer.baiduroadlayer.getVisible()) { BaseLayer.baiduroadlayer.setVisible(false); }
        if (BaseLayer.baiduimagelayer.getVisible() && BaseLayer.baidulabellayer.getVisible()) {
            BaseLayer.baiduimagelayer.setVisible(false);
            BaseLayer.baidulabellayer.setVisible(false);
        }
        if (BaseLayer.gaoderoadlayer.getVisible()) { BaseLayer.gaoderoadlayer.setVisible(false); }
        if (BaseLayer.gaodeimagelayer.getVisible() && BaseLayer.gaodelabellayer.getVisible()) {
            BaseLayer.gaodeimagelayer.setVisible(false);
            BaseLayer.gaodelabellayer.setVisible(false);
        }
        if (BaseLayer.tiandituroadLayer.getVisible() && BaseLayer.tianditulabelLayer.getVisible()) {
            BaseLayer.tiandituroadLayer.setVisible(false);
            BaseLayer.tianditulabelLayer.setVisible(false);
        }
        if (BaseLayer.tiandituimgLayer.getVisible() && BaseLayer.tianditulabelLayer.getVisible()) {
            BaseLayer.tiandituimgLayer.setVisible(false);
            BaseLayer.tianditulabelLayer.setVisible(false);
        }
        if (BaseLayer.googleroadLayer.getVisible()) { BaseLayer.googleroadLayer.setVisible(false); }
        if (BaseLayer.googleimgLayer.getVisible()) { BaseLayer.googleimgLayer.setVisible(false); }
        switch (type) {
            case 'baiduroad':
                BaseLayer.baiduroadlayer.setVisible(true);
                maptype = "baidu-road";
                break;
            case 'baiduimage':
                BaseLayer.baiduimagelayer.setVisible(true);
                BaseLayer.baidulabellayer.setVisible(true);
                maptype = "baidu-image";
                break;
            case 'gaoderoad':
                BaseLayer.gaoderoadlayer.setVisible(true);
                maptype = "gaode-road";
                break;
            case 'gaodeimage':
                BaseLayer.gaodeimagelayer.setVisible(true);
                BaseLayer.gaodelabellayer.setVisible(true);
                maptype = "gaode-image";
                break;
            case 'tiandituroad':
                BaseLayer.tiandituroadLayer.setVisible(true);
                BaseLayer.tianditulabelLayer.setVisible(true);
                maptype = "tdt-road";
                break;
            case 'tiandituimg':
                BaseLayer.tiandituimgLayer.setVisible(true);
                BaseLayer.tianditulabelLayer.setVisible(true);
                maptype = "tdt-image";
                break;
            case 'googleroad':
                BaseLayer.googleroadLayer.setVisible(true);
                maptype = "google-road";
                break;
            case 'googleimg':
                BaseLayer.googleimgLayer.setVisible(true);
                maptype = "google-road";
                break;
            default:
                BaseLayer.tiandituroadLayer.setVisible(true);
                BaseLayer.tianditulabelLayer.setVisible(true);
                maptype = "tdt-road";
        }
    },
    BaiduRoad: function () {//百度地图
        var projection = ol.proj.get("EPSG:3857");
        var resolutions = [];
        for (var i = 0; i < 20; i++) {
            resolutions[i] = Math.pow(2, 18 - i);
        }
        var tilegrid = new ol.tilegrid.TileGrid({
            origin: [0, 0],
            resolutions: resolutions
        });

        var baiduSource = new ol.source.TileImage({
            projection: projection,
            tileGrid: tilegrid,
            tileUrlFunction: function (tileCoord, pixelRatio, proj) {
                if (!tileCoord) {
                    return "";
                }
                var z = tileCoord[0];
                var x = tileCoord[1];
                var y = tileCoord[2];

                if (x < 0) {
                    x = "M" + (-x);
                }
                if (y < 0) {
                    y = "M" + (-y);
                }
                return BaseLayer.serverurl + "maptype=baidu-road&x="+x+"&y="+y+"&z="+z;
            }
        });

        var baiduroadlayer = new ol.layer.Tile({
            source: baiduSource
        });
        return baiduroadlayer;
    },
    BaiduImage: function () {//Bing地图
        var projection = ol.proj.get("EPSG:3857");
        var resolutions = [];
        for (var i = 0; i < 20; i++) {
            resolutions[i] = Math.pow(2, 18 - i);
        }
        var tilegrid = new ol.tilegrid.TileGrid({
            origin: [0, 0],
            resolutions: resolutions
        });

        var baiduSource = new ol.source.TileImage({
            projection: projection,
            tileGrid: tilegrid,
            tileUrlFunction: function (tileCoord, pixelRatio, proj) {
                if (!tileCoord) {
                    return "";
                }
                var z = tileCoord[0];
                var x = tileCoord[1];
                var y = tileCoord[2];

                if (x < 0) {
                    x = "M" + (-x);
                }
                if (y < 0) {
                    y = "M" + (-y);
                }
                return BaseLayer.serverurl + "maptype=baidu-image&x=" + x + "&y=" + y + "&z=" + z;
            }
        });

        var baiduimagelayer = new ol.layer.Tile({
            source: baiduSource
        });
        return baiduimagelayer;
    },
    BaiduLabel: function () {//Bing地图
        var projection = ol.proj.get("EPSG:3857");
        var resolutions = [];
        for (var i = 0; i < 20; i++) {
            resolutions[i] = Math.pow(2, 18 - i);
        }
        var tilegrid = new ol.tilegrid.TileGrid({
            origin: [0, 0],
            resolutions: resolutions
        });

        var baiduSource = new ol.source.TileImage({
            projection: projection,
            tileGrid: tilegrid,
            tileUrlFunction: function (tileCoord, pixelRatio, proj) {
                if (!tileCoord) {
                    return "";
                }
                var z = tileCoord[0];
                var x = tileCoord[1];
                var y = tileCoord[2];

                if (x < 0) {
                    x = "M" + (-x);
                }
                if (y < 0) {
                    y = "M" + (-y);
                }
                return BaseLayer.serverurl + "maptype=baidu-label&x=" + x + "&y=" + y + "&z=" + z;
            }
        });

        var baidulabellayer = new ol.layer.Tile({
            source: baiduSource
        });
        return baidulabellayer;
    },
    GaodeRoad: function () {//OpenStree地图
        var gaoderoadlayer = new ol.layer.Tile({
            source: new ol.source.XYZ({
                title: "高德地图",
                url: BaseLayer.serverurl + "maptype=gaode-road&x={x}&y={y}&z={z}"
            })
        });
        return gaoderoadlayer;
    },
    GaodeImage: function () {
        var gaodeimagelayer = new ol.layer.Tile({
            source: new ol.source.XYZ({
                title: "高德影像",
                url: BaseLayer.serverurl + "maptype=gaode-image&x={x}&y={y}&z={z}"
            })
        });
        return gaodeimagelayer;
    },
    GaodeLabel: function () {
        var gaodelabellayer = new ol.layer.Tile({
            source: new ol.source.XYZ({
                title: "高德标注",
                url: BaseLayer.serverurl + "maptype=gaode-label&x={x}&y={y}&z={z}"
            })
        });
        return gaodelabellayer;
    },
    TiandituRoadMap: function () {
        var tiandituroadLayer = new ol.layer.Tile({
            source: new ol.source.XYZ({
                title: "天地图路网",
                url: BaseLayer.serverurl + "maptype=tdt-road&x={x}&y={y}&z={z}"
            })
        });
        return tiandituroadLayer;
    },
    TiandituLabelMap: function () {
        var tianditulabelLayer = new ol.layer.Tile({
            title: "天地图文字标注",
            source: new ol.source.XYZ({
                url: BaseLayer.serverurl + "maptype=tdt-label&x={x}&y={y}&z={z}"
            })
        });
        return tianditulabelLayer;
    },
    TiandituImageMap: function () {
        var tiandituimageLayer = new ol.layer.Tile({
            title: "天地图卫星影像",
            source: new ol.source.XYZ({
                url: BaseLayer.serverurl + "maptype=tdt-image&x={x}&y={y}&z={z}"
            })
        });
        return tiandituimageLayer;
    },
    GoogleRoadMap: function () {
        var googleroadLayer = new ol.layer.Tile({
            title: "谷歌路线图",
            source: new ol.source.XYZ({
                url: BaseLayer.serverurl + "maptype=google-road&x={x}&y={y}&z={z}"
            })
        });
        return googleroadLayer;
    },
    GoogleImageMap: function () {
        var googleimageLayer = new ol.layer.Tile({
            title: "谷歌卫星图",
            source: new ol.source.XYZ({
                url: BaseLayer.serverurl + "maptype=google-image&x={x}&y={y}&z={z}"
            })
        });
        return googleimageLayer;
    }
}
