package com.SevenEleven.RelicKing.controller;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.service.MemberService;

import lombok.RequiredArgsConstructor;

@RestController
@RequestMapping("/api/members")
@RequiredArgsConstructor
public class MemberController {

	private final MemberService memberService;

	@GetMapping("/duplicate-email")
	public Response checkEmailForDuplicates(@RequestParam(value = "email") String email) {
		return memberService.checkEmailForDuplicates(email);
	}

	@GetMapping("/duplicate-nickname")
	public Response checkNicknameForDuplicates(@RequestParam(value = "nickname") String nickname) {
		return memberService.checkNicknameForDuplicates(nickname);
	}

}
