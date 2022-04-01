import { CommentVote } from './vote';
import { Post } from './post';
import { User } from './user';

export interface Comment {
  id: number;
  authorId: number;
  parentId?: number;
  postId: number;
  text: string;
  dateCreated: string;

  author?: User;
  parent?: Comment;
  post?: Post;

  children?: Comment[];
  votes?: CommentVote[];
}
