import { Coordinate } from "ol/coordinate";
import Map from "ol/Map";
import { Tile as TileLayer, Vector as VectorLayer } from "ol/layer";
import { XYZ, Vector as VectorSource } from "ol/source";
import { FullScreen, defaults as defaultControls } from "ol/control";
import View from "ol/View";
import Draw, { DrawEvent } from 'ol/interaction/Draw';
import Point from "ol/geom/Point";
import Feature from "ol/Feature";


export default class AddCommentViewModel {
  public longitude: KnockoutObservable<number>;
  public latitude: KnockoutObservable<number>;

  private _intl: Intl.NumberFormat;
  private _vectorSource: VectorSource<Feature<Point>>;
  private _map: Map;

  constructor(longitude: number, latitude: number, locale: string) {
    this.setPoint = this.setPoint.bind(this);

    this.longitude = ko.observable<number>(longitude);
    this.latitude = ko.observable<number>(latitude);

    this._intl = new Intl.NumberFormat(locale, { maximumFractionDigits: 10 });
    this._vectorSource = new VectorSource<Feature<Point>>();

    this._map = new Map({
      target: "map",
      layers: [
        new TileLayer({
          source: new XYZ({
            url: "https://mt0.google.com/vt/lyrs=m&hl=en&x={x}&y={y}&z={z}",
          }),
        }),
        new VectorLayer({
          source: this._vectorSource
        })
      ],
      view: new View({
        center: this.getCoordinate(),
        zoom: 4,
        projection: "EPSG:3857"
      }),
      controls: defaultControls().extend([new FullScreen()]),
    });

    this.restorePoint();
    this.addInteraction();
  }

  private getCoordinate(): Coordinate {
    const longitude = this.longitude();
    const latitude = this.latitude();

    return [longitude, latitude];
  }

  private addInteraction() {
    const drawInteraction = new Draw({
      source: this._vectorSource,
      type: "Point"
    });
    drawInteraction.on("drawend", this.setPoint);
    this._map.addInteraction(drawInteraction);
  }

  private setPoint({ feature }: DrawEvent) {
    this._vectorSource.clear(true);
    const geometry = feature.getGeometry();
    if (geometry instanceof Point) {
      const coordinate = geometry.getCoordinates();
      this.setCoordinate(coordinate);
    }
  }

  private setCoordinate(coordinates: Coordinate) {
    this.longitude(coordinates[0]);
    this.latitude(coordinates[1]);
  }

  private restorePoint() {
    const longitude = this.longitude();
    const latitude = this.latitude();

    const point = new Point([longitude, latitude]);
    const feature = new Feature(point);
    this._vectorSource.addFeature(feature);
  }

  public get localLongitude() {
    return this._intl.format(this.longitude()).replace(/\s/g, "");;
  }

  public get localLatitude() {
    return this._intl.format(this.latitude()).replace(/\s/g, "");
  }
}
