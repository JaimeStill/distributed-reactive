import { BehaviorSubject } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { SnackerService } from '../services';
import { ServerConfig } from '../config';

/*
  This class is still a work in progress.
  Its intent is to simplify the development of API
  services (see the ../apis directory).

  Beyond the standard get, getX, and post functions,
  the following functions have been added:

  assign: call a get and assign the result to the
          provided stream.

  assignText: call getText and assign the result to
              the provided stream.

  assignUrl: call getUrl and assign the result to
             the provided stream.

  retrieve: wrap a get call with a type of provided
            return data around a promise that resolves
            with the type of data that is received.

  action: wrap a get call with no return data around
          a boolean promise. See UploadApi.removeTopicImage.

  send: wrap a post call with a type of provided data
        around a promise that resolves with the type
        of data that is received. I.e. POST a Comment
        and return an integer representing the Id.
        NOTE: The send and received types can be the same.
        See CommentApi.saveComment.

  resolve: wrap a post call around a boolean promise. This
           indicates that there is no returned data, but
           provides a way to determine when the operation
           has completed successfully.

*/

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
    });

  assignText = (
    url: string,
    stream: BehaviorSubject<string>
  ) => this.getText(url)
    .subscribe({
      next: (data: string) => stream.next(data),
      error: this.snacker.error
    });

  assignUrl = (
    url: string,
    stream: BehaviorSubject<string>
  ) => this.getUrl(url)
    .subscribe({
      next: (data: string) => stream.next(data),
      error: this.snacker.error
    });

  retrieve = <T>(
    url: string
  ): Promise<T> => new Promise((res) => {
    this.get(url)
      .subscribe({
        next: (data: T) => res(data),
        error: (err: any) => {
          this.snacker.error(err);
          res(null)
        }
      })
  });

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
