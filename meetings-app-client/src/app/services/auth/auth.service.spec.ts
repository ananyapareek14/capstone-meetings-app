import { TestBed } from '@angular/core/testing';

import { AppAuthService } from './auth.service';

describe('AppAuthService', () => {
  let service: AppAuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AppAuthService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
