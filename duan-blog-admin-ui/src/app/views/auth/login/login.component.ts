import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {
  LoginRequest,
  AdminApiAuthApiClient,
  AuthenticatedResult,
} from '../../../api/admin-api.service.generated';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
  loginForm: FormGroup;
  constructor(
    private fb: FormBuilder,
    private authClient: AdminApiAuthApiClient,
    private alertService: AlertService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
    });
  }

  onLogin() {
    var loginRequest: LoginRequest = new LoginRequest({
      username: this.loginForm.controls['username'].value,
      password: this.loginForm.controls['password'].value,
    });
    this.authClient.login(loginRequest).subscribe({
      next: (response: AuthenticatedResult) => {
        // Save token and refresh token info to local storage
        //localStorage.setItem('token', response.t);
        //localStorage.setItem('refreshToken', response.refreshToken);
        this.router.navigate(['/dashboard']);
      },
      error: (error: any) => {
        console.error(error);
        this.alertService.showError('Đăng nhập thất bại');
      },
    });
  }
}
