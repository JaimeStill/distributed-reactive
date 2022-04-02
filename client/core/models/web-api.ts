import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { SnackerService } from '../services';
import { ServerConfig } from '../config';

export class WebApi {
  private endpoint: string
  constructor(
    private http: HttpClient,
    private config: ServerConfig,
    private snacker: SnackerService,
    private apiBase: string
  ) {
    this.endpoint = `${config.api}${apiBase.endsWith('/') ? apiBase : `${apiBase}/`}`;
  }

  private combineUrl = (url: string) => `${this.config.server}${url}`;

  get = <T>(url: string) => this.http.get<T>(`${this.endpoint}${url}`);
  getText = (url: string) => this.http.get(`${this.endpoint}${url}`, { responseType: 'text' });
  getUrl = (url: string) => this.getText(url)
    .pipe(
      map(this.combineUrl)
    );

  assign = <T>(
    url: string,
    stream: BehaviorSubject<T>
  ) => this.get<T>(url)
    .subscribe({
      next: (data: T) => stream.next(data),
      error: this.snacker.error
    })

  action = (
    url: string,
    next?: () => void
  ): Promise<boolean> => new Promise((resolve) => {
    this.get(url)
      .subscribe({
        next: () => {
          next && next();
          resolve(true);
        },
        error: (err: any) => {
          this.snacker.error(err);
          resolve(false);
        }
      })
  });

  post = <TSend, TReceive>(url: string, data: TSend) =>
    this.http.post<TReceive>(`${this.endpoint}${url}`, data);

  send = <TSend, TReceive>(
    url: string,
    data: TSend,
    next?: (data: TReceive) => void
  ): Promise<TReceive> => new Promise((res) => {
    this.post<TSend, TReceive>(url, data)
      .subscribe({
        next: (data: TReceive) => {
          next && next(data);
          res(data);
        },
        error: (err: any) => {
          this.snacker.error(err);
          res(null);
        }
      })
  });

  resolve = <T>(
    url: string,
    data: T,
    next?: () => void
  ): Promise<boolean> => new Promise((res) => {
    this.post(url, data)
      .subscribe({
        next: () => {
          next && next();
          res(true);
        },
        error: (err: any) => {
          this.snacker.error(err);
          res(false);
        }
      })
  });
}
