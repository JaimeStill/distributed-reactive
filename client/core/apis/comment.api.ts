import {
  Injectable,
  Optional
} from '@angular/core';

import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { Comment } from '../models';
import { SnackerService } from '../services';
import { SyncService } from '../services';
import { ServerConfig } from '../config';

@Injectable({
  providedIn: 'root'
})
export class CommentApi {
  constructor(
    private http: HttpClient,
    private snacker: SnackerService,
    private sync: SyncService,
    @Optional() private config: ServerConfig
  ) { }
}
