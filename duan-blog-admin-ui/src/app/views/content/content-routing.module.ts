import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PostComponent } from './posts/post.component';
import { AuthGuard } from '../../shared/auth.guard';
const routes: Routes = [
  {
    path: '',
    redirectTo: 'posts',
    pathMatch: 'full',
  },
  {
    path: 'posts',
    component: PostComponent,
    data: {
      title: 'Bài Viết',
      requiredPolicy: 'Permissions.Posts.View',
    },
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ContentRoutingModule {}
