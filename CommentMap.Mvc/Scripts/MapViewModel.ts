import View from "ol/View";
import type { Coordinate } from "ol/coordinate";
import Point from "ol/geom/Point";
import Feature from "ol/Feature";
import { Icon, Style } from "ol/style";
import type { Location } from "./types";

export class MapViewModel {
  private static NullCoordinate: Coordinate = [0, 0];

  private static IconStyle = new Style({
    image: new Icon({
      anchor: [0.5, 22],
      anchorXUnits: "fraction",
      anchorYUnits: "pixels",
      src: "/assets/marker.svg",
    }),
  });

  private constructor(private _coordinates: Coordinate[]) { }

  public static fromLocations(locations: readonly Location[]): MapViewModel {
    return new MapViewModel(locations.map(({ longitude, latitude }) => [longitude, latitude]));
  }

  private get first(): Coordinate {
    return this._coordinates.length > 0 ? this._coordinates[0] : MapViewModel.NullCoordinate;
  }

  public getViewAtFirst(): View {
    return new View({
      center: this.first,
      zoom: 15,
      projection: "EPSG:3857",
    });
  }

  public getPointFeatures(): Feature<Point>[] {
    return this._coordinates.map((coordinates) => {
      const feature = new Feature({
        geometry: new Point(coordinates),
      });
      feature.setStyle(MapViewModel.IconStyle);
      return feature;
    });
  }
}
