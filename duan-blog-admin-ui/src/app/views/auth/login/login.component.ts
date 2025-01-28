import { Component, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {
  LoginRequest,
  AdminApiAuthApiClient,
  AuthenticatedResult,
} from '../../../api/admin-api.service.generated';
import { AlertService } from '../../../shared/services/alert.service';
import { UrlConstants } from '../../../shared/constants/url.constant';
import { Router } from '@angular/router';
import { TokenStorageService } from '../../../shared/services/token.storage.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnDestroy {
  loginForm: FormGroup;
  private ngUnsubscribe: Subject<void> = new Subject<void>();
  loading = false;
  constructor(
    private fb: FormBuilder,
    private authClient: AdminApiAuthApiClient,
    private alertService: AlertService,
    private router: Router,
    private tokenStorageService: TokenStorageService
  ) {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }
  ngOnDestroy(): void {
    this.ngUnsubscribe.next();
    this.ngUnsubscribe.complete();
  }

  onLogin() {
    this.loading = true;
    var loginRequest: LoginRequest = new LoginRequest({
      username: this.loginForm.controls['username'].value,
      password: this.loginForm.controls['password'].value,
    });
    this.authClient
      .login(loginRequest)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe({
        next: (response: AuthenticatedResult) => {
          // Save token and refresh token info to local storage
          this.tokenStorageService.saveToken(response.token);
          this.tokenStorageService.saveRefreshToken(response.refreshToken);
          this.tokenStorageService.saveUser(response);
          this.loading = false;
          this.router.navigate([UrlConstants.HOME]);
        },
        error: (error: any) => {
          this.loading = false;
          console.error(error);
          this.alertService.showError('Đăng nhập thất bại');
        },
      });
  }
}
