import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import {
  AdminApiPostApiClient,
  PostActivityLogDto,
} from '../../../../api/admin-api.service.generated';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'app-post-activity-logs',
  templateUrl: './post-activity-logs.component.html',
  styleUrls: ['./post-activity-logs.component.scss'],
})
export class PostActivityLogsComponent implements OnInit, OnDestroy {
  //#region  variables
  private ngUnsubscribe = new Subject<void>();

  // Default
  public blockedPanelDetail: boolean = false;
  public title: string;
  public btnDisabled = false;
  public saveBtnName: string;
  public items: any[] = [];
  //#endregion
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private postApiClient: AdminApiPostApiClient
  ) {}
  ngOnDestroy(): void {
    if (this.ref) {
      this.ref.close();
    }
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }
  ngOnInit(): void {
    //Load data to form
    this.toggleBlockUI(true);
    this.postApiClient
      .getActivityLogs(this.config.data.id)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (repsonse: PostActivityLogDto[]) => {
          this.items = repsonse;
          this.toggleBlockUI(false);
        },
        error: () => {
          this.toggleBlockUI(false);
        },
      });
  }

  //#region  function
  //#endregion

  //#region  private
  private toggleBlockUI(enabled: boolean) {
    if (enabled == true) {
      this.btnDisabled = true;
      this.blockedPanelDetail = true;
    } else {
      setTimeout(() => {
        this.btnDisabled = false;
        this.blockedPanelDetail = false;
      }, 1000);
    }
  }
  //#endregion
}
