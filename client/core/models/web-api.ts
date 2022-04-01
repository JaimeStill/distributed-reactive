import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { ServerConfig } from '../config';

export class WebApi {
  private endpoint: string
  constructor(
    private http: HttpClient,
    private config: ServerConfig,
    private apiBase: string
  ) {
    this.endpoint = `${config.api}${apiBase.endsWith('/') ? apiBase : `${apiBase}/`}`;
  }

  private combineUrl = (url: string) => `${this.config.server}${url}`;

  streamData = <T>(url: string) => this.http.get<T>(`${this.endpoint}${url}`);
  streamText = (url: string) => this.http.get(`${this.endpoint}${url}`, {responseType: 'text'});
  streamUrl = (url: string) => this.streamText(url)
    .pipe(
      map(this.combineUrl)
    );
}
