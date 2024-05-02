package com.SevenEleven.RelicKing.controller;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import com.SevenEleven.RelicKing.common.response.Response;
import com.SevenEleven.RelicKing.dto.request.SignUpRequestDto;
import com.SevenEleven.RelicKing.service.MemberService;

import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;

@RestController
@RequestMapping("/api/members")
@RequiredArgsConstructor
public class MemberController {

	private final MemberService memberService;

	@PostMapping("/signup")
	public Response signup(@RequestBody @Valid SignUpRequestDto dto) {
		memberService.signup(dto);
		return new Response(HttpStatus.OK.value(), "회원가입이 완료되었습니다.", true);
	}

	// TODO : 로직 변경 필요
	@PostMapping("/reissue")
	public ResponseEntity<?> reissue(HttpServletRequest request, HttpServletResponse response) {
		return memberService.reissue(request, response);
	}

	@GetMapping("/duplicate-email")
	public Response checkEmailForDuplicates(@RequestParam(value = "email") String email) {
		return memberService.checkEmailForDuplicates(email);
	}

	@GetMapping("/duplicate-nickname")
	public Response checkNicknameForDuplicates(@RequestParam(value = "nickname") String nickname) {
		return memberService.checkNicknameForDuplicates(nickname);
	}

}
