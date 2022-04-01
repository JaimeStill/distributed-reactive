import { Comment } from './comment';
import { Post } from './post';
import { Topic } from './topic';
import { TopicUser } from './topic-user';
import { UserImage } from './upload';
import { Vote } from './vote';

export interface User {
  id: number;

  defaultPageSize: number;
  userDarkTheme: boolean;
  dateJoined: string;

  /*
    Properties mapped from ../ad-user.ts
  */
  guid: string;
  lastName: string;
  firstName: string;
  middleName: string;
  displayName: string;
  emailAddress: string;
  distinguishedName: string;
  samAccountName: string;
  userPrincipalName: string;
  voiceTelephoneNumber: string;
  socketName: string;

  image?: UserImage;

  comments?: Comment[];
  posts?: Post[];
  topics?: Topic[];
  memberships?: TopicUser[];
  votes?: Vote[];
}
