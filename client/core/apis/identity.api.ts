import {
  Injectable,
  Optional
} from '@angular/core';

import {
  AdUser,
  User,
  WebApi
} from '../models';

import {
  QueryGeneratorService,
  SnackerService,
  SyncService
} from '../services';

import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { ServerConfig } from '../config';

@Injectable({
  providedIn: 'root'
})
export class IdentityApi {
  api: WebApi;

  constructor(
    private generator: QueryGeneratorService,
    private http: HttpClient,
    private snacker: SnackerService,
    private sync: SyncService,
    @Optional() private config: ServerConfig
  ) {
    this.api = new WebApi(http, config, snacker, 'identity');
  }

  //#region AdUser

  private adUsers = new BehaviorSubject<AdUser[]>(null);
  adUsers$ = this.adUsers.asObservable();

  getDomainUsers = () =>
    this.api.assign(
      'getAdUsers',
      this.adUsers
    );

  searchDomainUsers = (search: string) =>
    this.api.assign(
      `searchDomainUsers/${search}`,
      this.adUsers
    );

  private currentAdUser = new BehaviorSubject<AdUser>(null);
  currentAdUser$ = this.currentAdUser.asObservable();

  getCurrentUser = () =>
    this.api.assign(
      'getCurrentUser',
      this.currentAdUser
    );

  //#endregion

  //#region User

  queryUsers = () =>
    this.generator.generateSource<User>(
      `lastName`,
      `user/queryUsers`
    );

  private user = new BehaviorSubject<User>(null);
  user$ = this.user.asObservable();

  getUser = (id: number) =>
    this.api.assign(
      `getUser/${id}`,
      this.user
    );

  getUserIdByGuid = () =>
    this.api.retrieve<number>(
      'getUserIdByGuid'
    );

  private currentUser = new BehaviorSubject<User>(null);
  currentUser$ = this.currentUser.asObservable();

  syncUser = () =>
    this.api.assign(
      'syncUser',
      this.currentUser
    );

  addUser = (adUser: AdUser) =>
    this.api.resolve(
      `addUser`,
      adUser,
      () => this.snacker.sendSuccessMessage(`Account ${adUser.displayName} successfully created`)
    );

  updateUser = (user: User) =>
    this.api.resolve(
      `updateUser`,
      user,
      () => this.snacker.sendSuccessMessage(`Account ${user.displayName} successfully updated`)
    );

  removeUser = (user: User) =>
    this.api.resolve(
      `removeUser`,
      user,
      () => this.snacker.sendSuccessMessage(`Account ${user.displayName} successfully removed`)
    )

  //#endregion
}
