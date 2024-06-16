import Map from "ol/Map";
import { Tile as TileLayer, Vector as VectorLayer } from "ol/layer";
import { XYZ, Vector as VectorSource } from "ol/source";
import { FullScreen, defaults as defaultControls } from "ol/control";
import { Coordinate } from "ol/coordinate";
import View from "ol/View";
import Feature from "ol/Feature";
import { Point } from "ol/geom";
import { Icon, Style } from "ol/style";


export default class CommentsViewModel {
  private static readonly MARKER_ICON_STYLE = new Style({
    image: new Icon({
      anchor: [0.5, 22],
      anchorXUnits: "fraction",
      anchorYUnits: "pixels",
      src: "/assets/marker.svg",
    }),
  });
  
  private _map: Map;

  constructor(private _coordinates: Coordinate[]) {
    this._map = new Map({
      target: "map",
      layers: [
        new TileLayer({
          source: new XYZ({
            url: "https://mt0.google.com/vt/lyrs=m&hl=en&x={x}&y={y}&z={z}"
          }),
        }),
        new VectorLayer({
          source: new VectorSource({
            features: this.getPointFeatures()
          }),
        })
      ],
      view: this.getViewAtFirst(),
      controls: defaultControls().extend([new FullScreen()]),
    });
  }
  
  private getViewAtFirst(): View {
    return new View({
      center: this.first,
      zoom: 15,
      projection: "EPSG:3857",
    });
  }

  private getPointFeatures(): Feature<Point>[] {
    return this._coordinates.map((coordinate) => {
      const feature = new Feature({
        geometry: new Point(coordinate),
      });
      feature.setStyle(CommentsViewModel.MARKER_ICON_STYLE);
      return feature;
    });
  }

  private get first(): Coordinate {
    return this._coordinates.length > 0 ? this._coordinates[0] : [0, 0];
  }

  private setView(coordinate: Coordinate) {
    const view = this._map.getView();
    view.setCenter(coordinate);
    view.setZoom(15);
  }

  public goToLocation(coordinate: Coordinate, viewModel: CommentsViewModel) {
    viewModel.setView(coordinate);
  }
}
