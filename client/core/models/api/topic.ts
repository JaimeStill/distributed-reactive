import { Post } from './post';
import { TopicImage } from './upload';
import { TopicUser } from './topic-user';
import { User } from './user';

export interface Topic {
  id: number;
  ownerId: number;
  name: string;
  url: string;
  dateCreated: string;

  owner?: User;

  image?: TopicImage;

  posts?: Post[];
  members?: TopicUser[];
}
