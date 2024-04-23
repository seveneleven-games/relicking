package com.SevenEleven.RelicKing.entity;

import java.time.LocalDate;

import org.springframework.data.annotation.LastModifiedDate;
import org.springframework.data.jpa.domain.support.AuditingEntityListener;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EntityListeners;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;

@Entity
@Getter
@Builder
@AllArgsConstructor
@NoArgsConstructor
@ToString
@EntityListeners(AuditingEntityListener.class)
public class Record {
	@Id
	@GeneratedValue(strategy = GenerationType.IDENTITY)
	private int recordId;

	@ManyToOne
	@JoinColumn(name = "member_id", referencedColumnName = "member_id", nullable = false)
	private Member member;

	@Column(nullable = false)
	private int stage;

	@Column(nullable = false)
	private int difficulty;

	@Column(nullable = false)
	private int eliteKill;

	@Column(nullable = false)
	private int normalKill;

	@Column(nullable = false)
	private int classNo;

	@LastModifiedDate
	@Column(nullable = false)
	@Builder.Default
	private LocalDate updatedDate = LocalDate.now();

}
