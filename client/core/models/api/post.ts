import { Comment } from './comment';
import { PostLink } from './post-link';
import { PostUpload } from './upload';
import { PostVote } from './vote';
import { Topic } from './topic';
import { User } from './user';

export interface Post {
  id: number;
  authorId: number;
  topicId: number;
  title: string;
  url: string;
  description: string;
  text: string;
  dateCreated: string;
  datePublished: string;
  isPublished: boolean;

  author?: User;
  topic?: Topic;

  comments?: Comment[];
  links?: PostLink[];
  uploads?: PostUpload[];
  votes?: PostVote[];
}
