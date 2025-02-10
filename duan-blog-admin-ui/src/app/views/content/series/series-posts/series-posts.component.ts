import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import {
  AddPostSeriesRequest,
  AdminApiSeriesApiClient,
  PostInListDto,
} from '../../../../api/admin-api.service.generated';
import { AlertService } from '../../../../shared/services/alert.service';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MessageConstants } from '../../../../shared/constants/message-constant';

@Component({
  selector: 'app-series-posts',
  templateUrl: './series-posts.component.html',
  styleUrls: ['./series-posts.component.scss'],
})
export class SeriesPostsComponent implements OnInit, OnDestroy {
  private ngUnsubscribe = new Subject<void>();

  public blockedPanel: boolean = false;
  public title: string;
  public posts: PostInListDto[] = [];

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private seriesApiClient: AdminApiSeriesApiClient,
    private alertService: AlertService
  ) {}

  ngOnInit() {
    this.initData();
  }
  initData() {
    this.toggleBlockUI(true);
    this.loadData(this.config.data?.id);
  }

  ngOnDestroy(): void {
    if (this.ref) {
      this.ref.close();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  loadData(id: string) {
    this.toggleBlockUI(true);

    this.seriesApiClient
      .getPostsInSeries(id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: PostInListDto[]) => {
          this.posts = response;
          this.toggleBlockUI(false);
        },
        error: (error) => {
          this.toggleBlockUI(false);
        },
      });
  }
  removePost(id: string) {
    var body: AddPostSeriesRequest = new AddPostSeriesRequest({
      postId: id,
      seriesId: this.config.data.id,
    });
    this.seriesApiClient
      .deletePostSeries(body)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: () => {
          this.alertService.showSuccess(MessageConstants.DELETED_OK_MSG);
          this.loadData(this.config.data?.id);
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }
  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.blockedPanel = true;
    } else {
      setTimeout(() => {
        this.blockedPanel = false;
      }, 1000);
    }
  }
}
