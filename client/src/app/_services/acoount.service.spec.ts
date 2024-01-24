import { TestBed } from '@angular/core/testing';

import { AcoountService } from './acoount.service';

describe('AcoountService', () => {
  let service: AcoountService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AcoountService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
